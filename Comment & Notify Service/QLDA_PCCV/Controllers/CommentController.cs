using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDA_PCCV.Domain.Notifications.Entities;
using QLDA_PCCV.Infrastructure.Persistence;
using QLDA_PCCV.Services;

namespace QLDA_PCCV.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly INotificationEventService _notificationEventService;

    public CommentController(AppDbContext context, INotificationEventService notificationEventService)
    {
        _context = context;
        _notificationEventService = notificationEventService;
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateComment([FromBody] CreateCommentRequest request)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        if (request.ParentCommentId.HasValue)
        {
            var parentComment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == request.ParentCommentId.Value);
            if (parentComment == null)
            {
                return NotFound(new { Message = $"Parent comment with ID {request.ParentCommentId} does not exist." });
            }

            if (parentComment.TaskId != request.TaskId)
            {
                return BadRequest(new { Message = "Parent comment must belong to the same task." });
            }
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            TaskId = request.TaskId,
            UserId = userId,
            Content = request.Content,
            ParentCommentId = request.ParentCommentId,
            IsDeleted = false,
            CreatedAt = DateTime.UtcNow
        };

        _context.Comments.Add(comment);

        // Optional: create activity log
        var activityLog = new ActivityLog
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TaskId = request.TaskId,
            Action = Domain.Notifications.Enums.ActivityAction.Commented,
            Detail = $"Added a comment on task: {request.Content.Substring(0, Math.Min(request.Content.Length, 50))}",
            CreatedAt = DateTime.UtcNow
        };
        _context.ActivityLogs.Add(activityLog);

        var createdMentionNotifications = await _notificationEventService.CreateCommentMentionNotificationsAsync(comment);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            comment.Id,
            comment.TaskId,
            comment.UserId,
            comment.Content,
            comment.ParentCommentId,
            comment.CreatedAt,
            CreatedMentionNotifications = createdMentionNotifications
        });
    }

    [HttpGet("task/{taskId}")]
    public async Task<IActionResult> GetCommentsForTask(Guid taskId)
    {
        var comments = await _context.Comments
            .Include(c => c.User)
            .Where(c => c.TaskId == taskId)
            .OrderBy(c => c.CreatedAt)
            .ToListAsync();

        var commentModels = comments.Select(c => new CommentDto
        {
            Id = c.Id,
            TaskId = c.TaskId,
            UserId = c.UserId,
            UserFullName = c.User?.FullName ?? "Unknown User",
            UserUsername = c.User?.Username ?? "unknown",
            Content = c.IsDeleted ? "[Comment deleted]" : c.Content,
            ParentCommentId = c.ParentCommentId,
            IsDeleted = c.IsDeleted,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToList();

        // Nest replies to their parents
        var rootComments = commentModels.Where(c => c.ParentCommentId == null).ToList();
        var replies = commentModels.Where(c => c.ParentCommentId != null).ToList();

        foreach (var root in rootComments)
        {
            AddReplies(root, replies);
        }

        return Ok(rootComments);
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment(Guid id)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == id);
        if (comment == null)
        {
            return NotFound(new { Message = "Comment not found." });
        }

        // Only author or admin can delete comment
        var userRoleString = User.FindFirst(ClaimTypes.Role)?.Value;
        var isAdmin = userRoleString == "Admin";

        if (comment.UserId != userId && !isAdmin)
        {
            return Forbid();
        }

        comment.IsDeleted = true;
        comment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Comment deleted successfully." });
    }

    private void AddReplies(CommentDto parent, List<CommentDto> allReplies)
    {
        parent.Replies = allReplies.Where(r => r.ParentCommentId == parent.Id).ToList();
        foreach (var reply in parent.Replies)
        {
            AddReplies(reply, allReplies);
        }
    }
}

public class CreateCommentRequest
{
    public Guid TaskId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
}

public class CommentDto
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public string UserFullName { get; set; } = string.Empty;
    public string UserUsername { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public List<CommentDto> Replies { get; set; } = new();
}
