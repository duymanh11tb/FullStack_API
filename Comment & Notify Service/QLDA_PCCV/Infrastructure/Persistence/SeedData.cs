using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using QLDA_PCCV.Domain.Notifications.Entities;
using QLDA_PCCV.Domain.Notifications.Enums;

namespace QLDA_PCCV.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync())
            return;

        var userId1 = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001");
        var userId2 = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000002");
        var userId3 = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000003");

        var projectId1 = Guid.Parse("b1b2c3d4-0002-0002-0002-000000000001");
        var taskId1 = Guid.Parse("d1b2c3d4-0004-0004-0004-000000000001");
        var taskId2 = Guid.Parse("d1b2c3d4-0004-0004-0004-000000000002");

        var now = DateTime.UtcNow;
        var passwordHash = HashPassword("Password123");

        var users = new List<User>
        {
            new() { Id = userId1, Email = "admin@gmail.com", Username = "admin", PasswordHash = passwordHash, FullName = "Nguyễn Văn Admin", Role = UserRole.Admin, IsActive = true, CreatedAt = now.AddDays(-60) },
            new() { Id = userId2, Email = "manh.nguyen@gmail.com", Username = "duymanh", PasswordHash = passwordHash, FullName = "Nguyễn Duy Mạnh", Role = UserRole.User, IsActive = true, CreatedAt = now.AddDays(-55) },
            new() { Id = userId3, Email = "linh.tran@gmail.com", Username = "tranailinh", PasswordHash = passwordHash, FullName = "Trần Ái Linh", Role = UserRole.User, IsActive = true, CreatedAt = now.AddDays(-50) }
        };
        context.Users.AddRange(users);

        var notificationSettings = users.Select(u => new NotificationSetting
        {
            UserId = u.Id, OnComment = true, OnAssigned = true, OnStatusChanged = true, OnMention = true, OnSprintStarted = true, OnMemberAdded = true
        }).ToList();
        context.NotificationSettings.AddRange(notificationSettings);

        var comments = new List<Comment>
        {
            new() { Id = Guid.NewGuid(), TaskId = taskId1, UserId = userId1, Content = "Hello world", IsDeleted = false, CreatedAt = now.AddDays(-10) },
            new() { Id = Guid.NewGuid(), TaskId = taskId2, UserId = userId2, Content = "Test comment", IsDeleted = false, CreatedAt = now.AddDays(-5) }
        };
        context.Comments.AddRange(comments);

        var activityLogs = new List<ActivityLog>
        {
            new() { Id = Guid.NewGuid(), TaskId = taskId1, ProjectId = projectId1, UserId = userId1, Action = ActivityAction.Assigned, Detail = "Assigned task", CreatedAt = now.AddDays(-29) },
        };
        context.ActivityLogs.AddRange(activityLogs);

        var notifications = new List<Notification>
        {
            new() { Id = Guid.NewGuid(), UserId = userId2, Type = NotificationType.TaskAssigned, Message = "Assigned task", ReferenceId = taskId1, ReferenceType = ReferenceType.Task, IsRead = true, CreatedAt = now.AddDays(-29) },
        };
        context.Notifications.AddRange(notifications);

        await context.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
