using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLDA_PCCV.Domain.Notifications.Entities;
using QLDA_PCCV.Infrastructure.Persistence;

namespace QLDA_PCCV.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class NotificationController : ControllerBase
{
    private readonly AppDbContext _context;

    public NotificationController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetNotifications([FromQuery] bool? isRead)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var query = _context.Notifications
            .Where(n => n.UserId == userId);

        if (isRead.HasValue)
        {
            query = query.Where(n => n.IsRead == isRead.Value);
        }

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Select(n => new
            {
                n.Id,
                n.Type,
                TypeName = n.Type.ToString(),
                n.Message,
                n.ReferenceId,
                ReferenceTypeName = n.ReferenceType.ToString(),
                n.IsRead,
                n.CreatedAt
            })
            .ToListAsync();

        return Ok(notifications);
    }

    [HttpPut("{id}/read")]
    public async Task<IActionResult> MarkAsRead(Guid id)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

        if (notification == null)
        {
            return NotFound(new { Message = "Notification not found." });
        }

        notification.IsRead = true;
        await _context.SaveChangesAsync();

        return Ok(new { Message = "Notification marked as read." });
    }

    [HttpPut("read-all")]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var unreadNotifications = await _context.Notifications
            .Where(n => n.UserId == userId && !n.IsRead)
            .ToListAsync();

        foreach (var notification in unreadNotifications)
        {
            notification.IsRead = true;
        }

        await _context.SaveChangesAsync();

        return Ok(new { Message = "All notifications marked as read." });
    }

    [HttpGet("settings")]
    public async Task<IActionResult> GetSettings()
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var settings = await _context.NotificationSettings
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (settings == null)
        {
            // Auto create settings if not exists
            settings = new NotificationSetting
            {
                UserId = userId,
                OnComment = true,
                OnAssigned = true,
                OnStatusChanged = true,
                OnMention = true,
                OnSprintStarted = true,
                OnMemberAdded = true
            };
            _context.NotificationSettings.Add(settings);
            await _context.SaveChangesAsync();
        }

        return Ok(new
        {
            settings.UserId,
            settings.OnComment,
            settings.OnAssigned,
            settings.OnStatusChanged,
            settings.OnMention,
            settings.OnSprintStarted,
            settings.OnMemberAdded
        });
    }

    [HttpPut("settings")]
    public async Task<IActionResult> UpdateSettings([FromBody] UpdateSettingsRequest request)
    {
        var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized();
        }

        var settings = await _context.NotificationSettings
            .FirstOrDefaultAsync(s => s.UserId == userId);

        if (settings == null)
        {
            settings = new NotificationSetting { UserId = userId };
            _context.NotificationSettings.Add(settings);
        }

        settings.OnComment = request.OnComment;
        settings.OnAssigned = request.OnAssigned;
        settings.OnStatusChanged = request.OnStatusChanged;
        settings.OnMention = request.OnMention;
        settings.OnSprintStarted = request.OnSprintStarted;
        settings.OnMemberAdded = request.OnMemberAdded;

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Notification settings updated successfully." });
    }
}

public class UpdateSettingsRequest
{
    public bool OnComment { get; set; }
    public bool OnAssigned { get; set; }
    public bool OnStatusChanged { get; set; }
    public bool OnMention { get; set; }
    public bool OnSprintStarted { get; set; }
    public bool OnMemberAdded { get; set; }
}
