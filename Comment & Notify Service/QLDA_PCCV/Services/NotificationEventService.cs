using Microsoft.EntityFrameworkCore;
using QLDA_PCCV.Domain.Notifications.Entities;
using QLDA_PCCV.Domain.Notifications.Enums;
using QLDA_PCCV.Infrastructure.Persistence;
using Microsoft.AspNetCore.SignalR;
using QLDA_PCCV.Hubs;
using System.Collections.Generic;
using System.Linq;

namespace QLDA_PCCV.Services;

public interface INotificationEventService
{
    Task<NotificationEventResult> ConsumeAsync(NotificationEventRequest request, CancellationToken cancellationToken = default);
    Task<int> CreateCommentMentionNotificationsAsync(Comment comment, CancellationToken cancellationToken = default);
}

public class NotificationEventService : INotificationEventService
{
    private readonly AppDbContext _context;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly IExternalServiceClient _externalServiceClient;

    public NotificationEventService(AppDbContext context, IHubContext<NotificationHub> hubContext, IExternalServiceClient externalServiceClient)
    {
        _context = context;
        _hubContext = hubContext;
        _externalServiceClient = externalServiceClient;
    }

    public async Task<NotificationEventResult> ConsumeAsync(NotificationEventRequest request, CancellationToken cancellationToken = default)
    {
        if (!TryMapEvent(request.EventType, out var notificationType, out var referenceType))
        {
            return NotificationEventResult.Invalid($"Unsupported event type: {request.EventType}");
        }

        var referenceId = ResolveReferenceId(request, referenceType);
        if (referenceId == Guid.Empty)
        {
            return NotificationEventResult.Invalid($"Reference id is required for {referenceType} events.");
        }

        if (notificationType == NotificationType.TaskAssigned)
        {
            var taskId = request.TaskId ?? referenceId;
            var task = taskId != Guid.Empty ? await _externalServiceClient.GetTaskAsync(taskId, cancellationToken) : null;
            var taskTitle = task?.Title ?? "công việc";
            var projectId = task?.BoardId ?? request.ProjectId ?? Guid.Empty;

            var actorId = request.ActorUserId ?? Guid.Empty;
            string actorName = "Một thành viên";
            if (actorId != Guid.Empty)
            {
                var actor = await _context.Users.FirstOrDefaultAsync(u => u.Id == actorId, cancellationToken);
                if (actor != null)
                {
                    actorName = actor.FullName;
                }
            }

            var assigneeId = request.TargetUserId ?? task?.AssigneeId ?? (request.RecipientUserIds != null && request.RecipientUserIds.Count > 0 ? request.RecipientUserIds.FirstOrDefault() : Guid.Empty);
            string assigneeName = "người dùng khác";
            var enabledAssigneeId = Guid.Empty;
            if (assigneeId != Guid.Empty)
            {
                var assigneeUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == assigneeId, cancellationToken);
                if (assigneeUser != null)
                {
                    assigneeName = assigneeUser.FullName;
                    
                    var enabledAssigneeIds = await GetUsersEnabledForAsync(new[] { assigneeId }, NotificationType.TaskAssigned, cancellationToken);
                    if (enabledAssigneeIds.Contains(assigneeId) && assigneeId != actorId)
                    {
                        enabledAssigneeId = assigneeId;
                    }
                }
            }

            // Extract tagged users
            var taggedUsernames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(request.Message))
            {
                taggedUsernames.UnionWith(ExtractMentionedUsernames(request.Message));
            }
            if (!string.IsNullOrEmpty(request.Detail))
            {
                taggedUsernames.UnionWith(ExtractMentionedUsernames(request.Detail));
            }
            if (task != null)
            {
                if (!string.IsNullOrEmpty(task.Title))
                {
                    taggedUsernames.UnionWith(ExtractMentionedUsernames(task.Title));
                }
                if (!string.IsNullOrEmpty(task.Description))
                {
                    taggedUsernames.UnionWith(ExtractMentionedUsernames(task.Description));
                }
            }

            var taggedUserIds = new HashSet<Guid>();
            if (taggedUsernames.Count > 0)
            {
                var users = await _context.Users
                    .Where(u => u.IsActive && taggedUsernames.Contains(u.Username))
                    .Select(u => u.Id)
                    .ToListAsync(cancellationToken);
                taggedUserIds.UnionWith(users);
            }

            var enabledTaggedUserIds = await GetUsersEnabledForAsync(taggedUserIds, NotificationType.CommentMention, cancellationToken);
            enabledTaggedUserIds.Remove(actorId);
            if (enabledAssigneeId != Guid.Empty)
            {
                enabledTaggedUserIds.Remove(enabledAssigneeId);
            }

            // Project members
            var projectMemberIds = new HashSet<Guid>();
            if (projectId != Guid.Empty)
            {
                var members = await _externalServiceClient.GetProjectMembersAsync(projectId, cancellationToken);
                if (members != null)
                {
                    projectMemberIds.UnionWith(members.Select(m => m.UserId).Where(id => id != Guid.Empty));
                }
            }

            // Ensure active local users
            var activeProjectMemberIds = new HashSet<Guid>();
            if (projectMemberIds.Count > 0)
            {
                var activeLocalMembers = await _context.Users
                    .Where(u => u.IsActive && projectMemberIds.Contains(u.Id))
                    .Select(u => u.Id)
                    .ToListAsync(cancellationToken);
                activeProjectMemberIds.UnionWith(activeLocalMembers);
            }

            var enabledProjectMemberIds = await GetUsersEnabledForAsync(activeProjectMemberIds, NotificationType.TaskAssigned, cancellationToken);
            enabledProjectMemberIds.Remove(actorId);
            if (enabledAssigneeId != Guid.Empty)
            {
                enabledProjectMemberIds.Remove(enabledAssigneeId);
            }
            foreach (var id in enabledTaggedUserIds)
            {
                enabledProjectMemberIds.Remove(id);
            }

            var assignmentNow = DateTime.UtcNow;
            var assignmentNotifications = new List<Notification>();

            // 1. Assignee
            if (enabledAssigneeId != Guid.Empty)
            {
                var messageText = request.Message ?? $"Bạn đã được {actorName} phân công vào công việc '{taskTitle}'.";
                var notif = new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = enabledAssigneeId,
                    Type = NotificationType.TaskAssigned,
                    Message = messageText,
                    ReferenceId = referenceId,
                    ReferenceType = referenceType,
                    IsRead = false,
                    CreatedAt = assignmentNow
                };
                _context.Notifications.Add(notif);
                assignmentNotifications.Add(notif);
            }

            // 2. Tagged users
            foreach (var userId in enabledTaggedUserIds)
            {
                var messageText = $"Bạn đã được nhắc đến trong công việc '{taskTitle}' bởi {actorName}.";
                var notif = new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = NotificationType.CommentMention,
                    Message = messageText,
                    ReferenceId = referenceId,
                    ReferenceType = referenceType,
                    IsRead = false,
                    CreatedAt = assignmentNow
                };
                _context.Notifications.Add(notif);
                assignmentNotifications.Add(notif);
            }

            // 3. Project members
            foreach (var userId in enabledProjectMemberIds)
            {
                var messageText = $"{actorName} đã giao công việc '{taskTitle}' cho {assigneeName}.";
                var notif = new Notification
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    Type = NotificationType.TaskAssigned,
                    Message = messageText,
                    ReferenceId = referenceId,
                    ReferenceType = referenceType,
                    IsRead = false,
                    CreatedAt = assignmentNow
                };
                _context.Notifications.Add(notif);
                assignmentNotifications.Add(notif);
            }

            AddActivityLogIfNeeded(request, notificationType, assignmentNow);

            await _context.SaveChangesAsync(cancellationToken);

            // Push SignalR notifications
            foreach (var notif in assignmentNotifications)
            {
                try
                {
                    await _hubContext.Clients.Group(notif.UserId.ToString().ToLower()).SendAsync("ReceiveNotification", new
                    {
                        id = notif.Id,
                        userId = notif.UserId,
                        type = notif.Type.ToString(),
                        message = notif.Message,
                        referenceId = notif.ReferenceId,
                        referenceType = notif.ReferenceType.ToString(),
                        isRead = notif.IsRead,
                        createdAt = notif.CreatedAt
                    });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error pushing real-time notification: {ex.Message}");
                }
            }

            return NotificationEventResult.Success(assignmentNotifications.Count, "Event consumed and notifications created.");
        }

        var recipients = await ResolveRecipientsAsync(request, notificationType, cancellationToken);
        if (recipients.Count == 0)
        {
            return NotificationEventResult.Success(0, "Event consumed, but no eligible recipients were found.");
        }

        var message = await ResolveMessageAsync(request, notificationType, referenceId, cancellationToken);
        var now = DateTime.UtcNow;
        var createdNotifications = new List<Notification>();

        foreach (var userId in recipients)
        {
            var notif = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Type = notificationType,
                Message = message,
                ReferenceId = referenceId,
                ReferenceType = referenceType,
                IsRead = false,
                CreatedAt = now
            };
            _context.Notifications.Add(notif);
            createdNotifications.Add(notif);
        }

        AddActivityLogIfNeeded(request, notificationType, now);

        await _context.SaveChangesAsync(cancellationToken);

        // Push real-time notifications
        foreach (var notif in createdNotifications)
        {
            try
            {
                await _hubContext.Clients.Group(notif.UserId.ToString().ToLower()).SendAsync("ReceiveNotification", new
                {
                    id = notif.Id,
                    userId = notif.UserId,
                    type = notif.Type.ToString(),
                    message = notif.Message,
                    referenceId = notif.ReferenceId,
                    referenceType = notif.ReferenceType.ToString(),
                    isRead = notif.IsRead,
                    createdAt = notif.CreatedAt
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error pushing real-time notification: {ex.Message}");
            }
        }

        return NotificationEventResult.Success(recipients.Count, "Event consumed and notifications created.");
    }

    public async Task<int> CreateCommentMentionNotificationsAsync(Comment comment, CancellationToken cancellationToken = default)
    {
        var usernames = ExtractMentionedUsernames(comment.Content);
        if (usernames.Count == 0)
        {
            return 0;
        }

        var mentionedUsers = await _context.Users
            .Where(u => u.IsActive && usernames.Contains(u.Username) && u.Id != comment.UserId)
            .Select(u => new { u.Id, u.Username })
            .ToListAsync(cancellationToken);

        if (mentionedUsers.Count == 0)
        {
            return 0;
        }

        var enabledUserIds = await GetUsersEnabledForAsync(
            mentionedUsers.Select(u => u.Id),
            NotificationType.CommentMention,
            cancellationToken);

        var now = DateTime.UtcNow;
        var createdNotifs = new List<Notification>();

        foreach (var user in mentionedUsers.Where(u => enabledUserIds.Contains(u.Id)))
        {
            var notif = new Notification
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Type = NotificationType.CommentMention,
                Message = $"Bạn đã được nhắc đến trong một bình luận công việc.",
                ReferenceId = comment.TaskId,
                ReferenceType = ReferenceType.Task,
                IsRead = false,
                CreatedAt = now
            };
            _context.Notifications.Add(notif);
            createdNotifs.Add(notif);
        }

        await _context.SaveChangesAsync(cancellationToken);

        foreach (var notif in createdNotifs)
        {
            try
            {
                await _hubContext.Clients.Group(notif.UserId.ToString().ToLower()).SendAsync("ReceiveNotification", new
                {
                    id = notif.Id,
                    userId = notif.UserId,
                    type = notif.Type.ToString(),
                    message = notif.Message,
                    referenceId = notif.ReferenceId,
                    referenceType = notif.ReferenceType.ToString(),
                    isRead = notif.IsRead,
                    createdAt = notif.CreatedAt
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error pushing real-time notification: {ex.Message}");
            }
        }

        return enabledUserIds.Count;
    }

    private async Task<HashSet<Guid>> ResolveRecipientsAsync(
        NotificationEventRequest request,
        NotificationType notificationType,
        CancellationToken cancellationToken)
    {
        var recipientIds = request.RecipientUserIds
            .Concat(request.TargetUserId.HasValue ? new[] { request.TargetUserId.Value } : Array.Empty<Guid>())
            .Where(id => id != Guid.Empty)
            .ToHashSet();

        if (notificationType == NotificationType.SprintStarted && recipientIds.Count == 0 && request.ProjectId.HasValue)
        {
            // Project Members must be provided by the event publisher since we don't have access to the Project API here.
        }

        if (recipientIds.Count == 0)
        {
            return recipientIds;
        }

        var activeUserIds = await _context.Users
            .Where(u => u.IsActive && recipientIds.Contains(u.Id))
            .Select(u => u.Id)
            .ToListAsync(cancellationToken);

        return await GetUsersEnabledForAsync(activeUserIds, notificationType, cancellationToken);
    }

    private async Task<HashSet<Guid>> GetUsersEnabledForAsync(
        IEnumerable<Guid> userIds,
        NotificationType notificationType,
        CancellationToken cancellationToken)
    {
        var ids = userIds.ToHashSet();
        if (ids.Count == 0)
        {
            return ids;
        }

        var settings = await _context.NotificationSettings
            .Where(s => ids.Contains(s.UserId))
            .ToListAsync(cancellationToken);

        var disabledIds = settings
            .Where(s => !IsEnabled(s, notificationType))
            .Select(s => s.UserId)
            .ToList();

        ids.ExceptWith(disabledIds);
        return ids;
    }

    private async Task<string> ResolveMessageAsync(
        NotificationEventRequest request,
        NotificationType notificationType,
        Guid referenceId,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.Message))
        {
            return request.Message.Trim();
        }

        return notificationType switch
        {
            NotificationType.TaskAssigned => "Bạn đã được phân công vào một công việc mới.",
            NotificationType.TaskStatusChanged => "Trạng thái của một công việc đã thay đổi.",
            NotificationType.CommentMention => "Bạn đã được nhắc đến trong một bình luận công việc.",
            NotificationType.SprintStarted => "Một Sprint mới đã bắt đầu.",
            NotificationType.MemberAdded => "Bạn đã được thêm vào một dự án.",
            NotificationType.TaskDeadlineApproaching => request.EventType.Contains("overdue") ? "Công việc của bạn đã quá hạn hoàn thành!" : "Công việc của bạn sắp đến hạn hoàn thành!",
            NotificationType.CommentAdded => "Có bình luận mới trong công việc của bạn.",
            _ => "Bạn có một thông báo mới."
        };
    }

    private void AddActivityLogIfNeeded(NotificationEventRequest request, NotificationType notificationType, DateTime now)
    {
        var normalizedEvent = NormalizeEventType(request.EventType);
        var action = normalizedEvent switch
        {
            "task.created" => ActivityAction.TaskCreated,
            "task.updated" => ActivityAction.TaskUpdated,
            "task.assigned" => ActivityAction.Assigned,
            "task.status.changed" => ActivityAction.StatusChanged,
            "member.added" => ActivityAction.MemberAdded,
            _ => (ActivityAction?)null
        };

        if (!action.HasValue || !request.ActorUserId.HasValue || !request.TaskId.HasValue)
        {
            return;
        }

        _context.ActivityLogs.Add(new ActivityLog
        {
            Id = Guid.NewGuid(),
            UserId = request.ActorUserId.Value,
            TaskId = request.TaskId.Value,
            ProjectId = request.ProjectId,
            Action = action.Value,
            Detail = request.Detail ?? request.Message,
            CreatedAt = now
        });
    }

    private static bool TryMapEvent(string eventType, out NotificationType notificationType, out ReferenceType referenceType)
    {
        switch (NormalizeEventType(eventType))
        {
            case "task.created":
                notificationType = NotificationType.TaskStatusChanged;
                referenceType = ReferenceType.Task;
                return true;
            case "task.updated":
                notificationType = NotificationType.TaskStatusChanged;
                referenceType = ReferenceType.Task;
                return true;
            case "task.assigned":
                notificationType = NotificationType.TaskAssigned;
                referenceType = ReferenceType.Task;
                return true;
            case "task.status.changed":
                notificationType = NotificationType.TaskStatusChanged;
                referenceType = ReferenceType.Task;
                return true;
            case "comment.mention":
                notificationType = NotificationType.CommentMention;
                referenceType = ReferenceType.Task;
                return true;
            case "sprint.started":
                notificationType = NotificationType.SprintStarted;
                referenceType = ReferenceType.Sprint;
                return true;
            case "member.added":
                notificationType = NotificationType.MemberAdded;
                referenceType = ReferenceType.Project;
                return true;
            case "task.deadline.approaching":
            case "task.deadline.overdue":
                notificationType = NotificationType.TaskDeadlineApproaching;
                referenceType = ReferenceType.Task;
                return true;
            case "comment.added":
                notificationType = NotificationType.CommentAdded;
                referenceType = ReferenceType.Task;
                return true;
            default:
                notificationType = default;
                referenceType = default;
                return false;
        }
    }

    private static Guid ResolveReferenceId(NotificationEventRequest request, ReferenceType referenceType) => referenceType switch
    {
        ReferenceType.Task => request.TaskId ?? request.ReferenceId,
        ReferenceType.Project => request.ProjectId ?? request.ReferenceId,
        ReferenceType.Sprint => request.SprintId ?? request.ReferenceId,
        _ => request.ReferenceId
    } ?? Guid.Empty;

    private static bool IsEnabled(NotificationSetting setting, NotificationType type) => type switch
    {
        NotificationType.TaskAssigned => setting.OnAssigned,
        NotificationType.TaskStatusChanged => setting.OnStatusChanged,
        NotificationType.CommentMention => setting.OnMention,
        NotificationType.SprintStarted => setting.OnSprintStarted,
        NotificationType.MemberAdded => setting.OnMemberAdded,
        NotificationType.CommentAdded => setting.OnComment,
        NotificationType.TaskDeadlineApproaching => true,
        _ => true
    };

    private static HashSet<string> ExtractMentionedUsernames(string content)
    {
        var usernames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (System.Text.RegularExpressions.Match match in System.Text.RegularExpressions.Regex.Matches(content, @"@([A-Za-z0-9_.-]{3,100})"))
        {
            usernames.Add(match.Groups[1].Value);
        }

        return usernames;
    }

    private static string NormalizeEventType(string eventType) => eventType.Trim().ToLowerInvariant().Replace('_', '.');
}

public class NotificationEventRequest
{
    public string EventType { get; set; } = string.Empty;
    public Guid? ReferenceId { get; set; }
    public Guid? TaskId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? SprintId { get; set; }
    public Guid? ActorUserId { get; set; }
    public Guid? TargetUserId { get; set; }
    public List<Guid> RecipientUserIds { get; set; } = new();
    public string? Message { get; set; }
    public string? Detail { get; set; }
}

public record NotificationEventResult(bool IsValid, int CreatedNotifications, string Message)
{
    public static NotificationEventResult Success(int createdNotifications, string message) => new(true, createdNotifications, message);
    public static NotificationEventResult Invalid(string message) => new(false, 0, message);
}
