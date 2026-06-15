using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using QLDA_PCCV.Domain.Notifications.Entities;
using QLDA_PCCV.Domain.Notifications.Enums;
using QLDA_PCCV.Domain.ProjectManagement.Entities;
using QLDA_PCCV.Domain.ProjectManagement.Enums;
using QLDA_PCCV.Domain.TaskManagement.Entities;
using QLDA_PCCV.Domain.TaskManagement.Enums;

namespace QLDA_PCCV.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext context)
    {
        // Only seed if no users exist
        if (await context.Users.AnyAsync())
            return;

        // ── Fixed GUIDs ──────────────────────────────────────────────
        // Users
        var userId1 = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000001");
        var userId2 = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000002");
        var userId3 = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000003");
        var userId4 = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000004");
        var userId5 = Guid.Parse("a1b2c3d4-0001-0001-0001-000000000005");

        // Projects
        var projectId1 = Guid.Parse("b1b2c3d4-0002-0002-0002-000000000001");
        var projectId2 = Guid.Parse("b1b2c3d4-0002-0002-0002-000000000002");
        var projectId3 = Guid.Parse("b1b2c3d4-0002-0002-0002-000000000003");

        // Sprints
        var sprintId1 = Guid.Parse("c1b2c3d4-0003-0003-0003-000000000001");
        var sprintId2 = Guid.Parse("c1b2c3d4-0003-0003-0003-000000000002");
        var sprintId3 = Guid.Parse("c1b2c3d4-0003-0003-0003-000000000003");
        var sprintId4 = Guid.Parse("c1b2c3d4-0003-0003-0003-000000000004");

        // Tasks
        var taskId1 = Guid.Parse("d1b2c3d4-0004-0004-0004-000000000001");
        var taskId2 = Guid.Parse("d1b2c3d4-0004-0004-0004-000000000002");
        var taskId3 = Guid.Parse("d1b2c3d4-0004-0004-0004-000000000003");
        var taskId4 = Guid.Parse("d1b2c3d4-0004-0004-0004-000000000004");
        var taskId5 = Guid.Parse("d1b2c3d4-0004-0004-0004-000000000005");
        var taskId6 = Guid.Parse("d1b2c3d4-0004-0004-0004-000000000006");
        var taskId7 = Guid.Parse("d1b2c3d4-0004-0004-0004-000000000007");
        var taskId8 = Guid.Parse("d1b2c3d4-0004-0004-0004-000000000008");
        var taskId9 = Guid.Parse("d1b2c3d4-0004-0004-0004-000000000009");
        var taskId10 = Guid.Parse("d1b2c3d4-0004-0004-0004-000000000010");

        // Comments
        var commentId1 = Guid.Parse("e1b2c3d4-0005-0005-0005-000000000001");
        var commentId2 = Guid.Parse("e1b2c3d4-0005-0005-0005-000000000002");
        var commentId3 = Guid.Parse("e1b2c3d4-0005-0005-0005-000000000003");
        var commentId4 = Guid.Parse("e1b2c3d4-0005-0005-0005-000000000004");
        var commentId5 = Guid.Parse("e1b2c3d4-0005-0005-0005-000000000005");
        var commentId6 = Guid.Parse("e1b2c3d4-0005-0005-0005-000000000006");
        var commentId7 = Guid.Parse("e1b2c3d4-0005-0005-0005-000000000007");
        var commentId8 = Guid.Parse("e1b2c3d4-0005-0005-0005-000000000008");

        // Milestones
        var milestoneId1 = Guid.Parse("f1b2c3d4-0006-0006-0006-000000000001");
        var milestoneId2 = Guid.Parse("f1b2c3d4-0006-0006-0006-000000000002");
        var milestoneId3 = Guid.Parse("f1b2c3d4-0006-0006-0006-000000000003");
        var milestoneId4 = Guid.Parse("f1b2c3d4-0006-0006-0006-000000000004");

        var now = DateTime.UtcNow;

        // ══════════════════════════════════════════════════════════════
        // 1. USERS (5 users – 1 Admin, 4 Users)
        //    Password for all: "Password123"
        // ══════════════════════════════════════════════════════════════
        var passwordHash = HashPassword("Password123");

        var users = new List<User>
        {
            new()
            {
                Id = userId1,
                Email = "admin@gmail.com",
                Username = "admin",
                PasswordHash = passwordHash,
                FullName = "Nguyễn Văn Admin",
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = now.AddDays(-60)
            },
            new()
            {
                Id = userId2,
                Email = "manh.nguyen@gmail.com",
                Username = "duymanh",
                PasswordHash = passwordHash,
                FullName = "Nguyễn Duy Mạnh",
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = now.AddDays(-55)
            },
            new()
            {
                Id = userId3,
                Email = "linh.tran@gmail.com",
                Username = "tranailinh",
                PasswordHash = passwordHash,
                FullName = "Trần Ái Linh",
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = now.AddDays(-50)
            },
            new()
            {
                Id = userId4,
                Email = "hoang.le@gmail.com",
                Username = "lehoang",
                PasswordHash = passwordHash,
                FullName = "Lê Minh Hoàng",
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = now.AddDays(-45)
            },
            new()
            {
                Id = userId5,
                Email = "thao.pham@gmail.com",
                Username = "phamthao",
                PasswordHash = passwordHash,
                FullName = "Phạm Thanh Thảo",
                Role = UserRole.User,
                IsActive = true,
                CreatedAt = now.AddDays(-40)
            }
        };

        context.Users.AddRange(users);

        // ── Notification Settings for all users ──
        var notificationSettings = users.Select(u => new NotificationSetting
        {
            UserId = u.Id,
            OnComment = true,
            OnAssigned = true,
            OnStatusChanged = true,
            OnMention = true,
            OnSprintStarted = true,
            OnMemberAdded = true
        }).ToList();

        context.NotificationSettings.AddRange(notificationSettings);


        // ══════════════════════════════════════════════════════════════
        // 10. COMMENTS (8 comments with nested replies)
        // ══════════════════════════════════════════════════════════════
        var comments = new List<Comment>
        {
            // Task 3 – Comment thread
            new()
            {
                Id = commentId1,
                TaskId = taskId3,
                UserId = userId1,
                Content = "Mạnh ơi, phần nested comment nên giới hạn tối đa bao nhiêu cấp? Mình nghĩ nên giới hạn 3 cấp để tránh UI bị xấu.",
                ParentCommentId = null,
                IsDeleted = false,
                CreatedAt = now.AddDays(-10)
            },
            new()
            {
                Id = commentId2,
                TaskId = taskId3,
                UserId = userId2,
                Content = "Em nghĩ 3 cấp là hợp lý anh ạ. Nếu sâu hơn thì sẽ flat lại và hiển thị \"replying to @username\". Em sẽ handle phần này trong API.",
                ParentCommentId = commentId1,
                IsDeleted = false,
                CreatedAt = now.AddDays(-10).AddHours(2)
            },
            new()
            {
                Id = commentId3,
                TaskId = taskId3,
                UserId = userId3,
                Content = "Đồng ý với phương án này. Mình cũng cần thêm tính năng mention user trong comment bằng ký tự @, để trigger notification cho người được mention.",
                ParentCommentId = commentId1,
                IsDeleted = false,
                CreatedAt = now.AddDays(-9)
            },

            // Task 7 – Comment thread
            new()
            {
                Id = commentId4,
                TaskId = taskId7,
                UserId = userId2,
                Content = "Linh ơi, phần giỏ hàng đã xong API chưa? Mình cần review trước khi merge vào develop branch.",
                ParentCommentId = null,
                IsDeleted = false,
                CreatedAt = now.AddDays(-5)
            },
            new()
            {
                Id = commentId5,
                TaskId = taskId7,
                UserId = userId3,
                Content = "Em đã xong rồi anh, đang ở trạng thái Review. Anh có thể review PR #42 trên GitHub. Em đã viết unit test đạt 85% coverage.",
                ParentCommentId = commentId4,
                IsDeleted = false,
                CreatedAt = now.AddDays(-5).AddHours(3)
            },

            // Task 9 – Comment thread
            new()
            {
                Id = commentId6,
                TaskId = taskId9,
                UserId = userId1,
                Content = "Hoàng ơi, Twilio đang bị rate limit khá nhiều ở VN. Em thử test lại với Firebase Auth xem có ổn định hơn không nhé.",
                ParentCommentId = null,
                IsDeleted = false,
                CreatedAt = now.AddDays(-4)
            },
            new()
            {
                Id = commentId7,
                TaskId = taskId9,
                UserId = userId4,
                Content = "Dạ em sẽ thử Firebase Auth. Nhưng Firebase SMS Verification cũng có giới hạn miễn phí chỉ 10k/tháng. Em sẽ làm so sánh chi phí 2 bên gửi anh review.",
                ParentCommentId = commentId6,
                IsDeleted = false,
                CreatedAt = now.AddDays(-4).AddHours(1)
            },

            // Task 6 – standalone comment
            new()
            {
                Id = commentId8,
                TaskId = taskId6,
                UserId = userId2,
                Content = "Thảo ơi, phần responsive mobile đang bị lỗi layout trên iPhone SE. Em check lại breakpoint 375px nhé. Mình cần fix trước khi close sprint.",
                ParentCommentId = null,
                IsDeleted = false,
                CreatedAt = now.AddDays(-3)
            }
        };

        context.Comments.AddRange(comments);

        // ══════════════════════════════════════════════════════════════
        // 11. ACTIVITY LOGS
        // ══════════════════════════════════════════════════════════════
        var activityLogs = new List<ActivityLog>
        {
            new() { Id = Guid.NewGuid(), TaskId = taskId1, ProjectId = projectId1, UserId = userId1, Action = ActivityAction.Assigned, Detail = "Đã assign task \"Thiết kế ERD Database\" cho Nguyễn Duy Mạnh", CreatedAt = now.AddDays(-29) },
            new() { Id = Guid.NewGuid(), TaskId = taskId1, ProjectId = projectId1, UserId = userId2, Action = ActivityAction.StatusChanged, Detail = "Chuyển trạng thái từ \"Backlog\" sang \"In Progress\"", CreatedAt = now.AddDays(-26) },
            new() { Id = Guid.NewGuid(), TaskId = taskId1, ProjectId = projectId1, UserId = userId2, Action = ActivityAction.StatusChanged, Detail = "Chuyển trạng thái từ \"In Progress\" sang \"Done\"", CreatedAt = now.AddDays(-18) },
            new() { Id = Guid.NewGuid(), TaskId = taskId2, ProjectId = projectId1, UserId = userId1, Action = ActivityAction.Assigned, Detail = "Đã assign task \"Viết API CRUD cho Project\" cho Trần Ái Linh", CreatedAt = now.AddDays(-28) },
            new() { Id = Guid.NewGuid(), TaskId = taskId3, ProjectId = projectId1, UserId = userId1, Action = ActivityAction.Commented, Detail = "Đã comment về giới hạn cấp nested comment", CreatedAt = now.AddDays(-10) },
            new() { Id = Guid.NewGuid(), TaskId = taskId3, ProjectId = projectId1, UserId = userId2, Action = ActivityAction.StatusChanged, Detail = "Chuyển trạng thái từ \"To Do\" sang \"In Progress\"", CreatedAt = now.AddDays(-12) },
            new() { Id = Guid.NewGuid(), TaskId = taskId7, ProjectId = projectId2, UserId = userId3, Action = ActivityAction.StatusChanged, Detail = "Chuyển trạng thái từ \"In Progress\" sang \"Review\"", CreatedAt = now.AddDays(-1) },
            new() { Id = Guid.NewGuid(), TaskId = taskId9, ProjectId = projectId3, UserId = userId4, Action = ActivityAction.StatusChanged, Detail = "Chuyển trạng thái từ \"To Do\" sang \"In Progress\"", CreatedAt = now.AddDays(-6) },
            new() { Id = Guid.NewGuid(), TaskId = Guid.Empty, ProjectId = projectId1, UserId = userId1, Action = ActivityAction.MemberAdded, Detail = "Đã thêm Phạm Thanh Thảo vào dự án với role Viewer", CreatedAt = now.AddDays(-26) },
            new() { Id = Guid.NewGuid(), TaskId = Guid.Empty, ProjectId = projectId3, UserId = userId1, Action = ActivityAction.MemberAdded, Detail = "Đã thêm Trần Ái Linh vào dự án Mobile Banking", CreatedAt = now.AddDays(-12) }
        };

        context.ActivityLogs.AddRange(activityLogs);

        // ══════════════════════════════════════════════════════════════
        // 12. NOTIFICATIONS (various types)
        // ══════════════════════════════════════════════════════════════
        var notifications = new List<Notification>
        {
            // Task assigned notifications
            new() { Id = Guid.NewGuid(), UserId = userId2, Type = NotificationType.TaskAssigned, Message = "Bạn đã được assign task \"Thiết kế ERD Database\" trong dự án QLDA PCCV.", ReferenceId = taskId1, ReferenceType = ReferenceType.Task, IsRead = true, CreatedAt = now.AddDays(-29) },
            new() { Id = Guid.NewGuid(), UserId = userId3, Type = NotificationType.TaskAssigned, Message = "Bạn đã được assign task \"Viết API CRUD cho Project\" trong dự án QLDA PCCV.", ReferenceId = taskId2, ReferenceType = ReferenceType.Task, IsRead = true, CreatedAt = now.AddDays(-28) },
            new() { Id = Guid.NewGuid(), UserId = userId4, Type = NotificationType.TaskAssigned, Message = "Bạn đã được assign task \"Tích hợp Notification Service\".", ReferenceId = taskId4, ReferenceType = ReferenceType.Task, IsRead = false, CreatedAt = now.AddDays(-12) },
            new() { Id = Guid.NewGuid(), UserId = userId4, Type = NotificationType.TaskAssigned, Message = "Bạn đã được assign task \"Thiết kế luồng đăng ký OTP\" trong dự án Mobile Banking.", ReferenceId = taskId9, ReferenceType = ReferenceType.Task, IsRead = true, CreatedAt = now.AddDays(-8) },

            // Status changed notifications
            new() { Id = Guid.NewGuid(), UserId = userId1, Type = NotificationType.TaskStatusChanged, Message = "Task \"Thiết kế ERD Database\" đã chuyển sang trạng thái Done.", ReferenceId = taskId1, ReferenceType = ReferenceType.Task, IsRead = true, CreatedAt = now.AddDays(-18) },
            new() { Id = Guid.NewGuid(), UserId = userId2, Type = NotificationType.TaskStatusChanged, Message = "Task \"Xây dựng API quản lý giỏ hàng\" đã chuyển sang Review.", ReferenceId = taskId7, ReferenceType = ReferenceType.Task, IsRead = false, CreatedAt = now.AddDays(-1) },

            // Comment mention notifications
            new() { Id = Guid.NewGuid(), UserId = userId2, Type = NotificationType.CommentMention, Message = "Nguyễn Văn Admin đã comment trong task \"Xây dựng hệ thống Comment nested\".", ReferenceId = taskId3, ReferenceType = ReferenceType.Task, IsRead = true, CreatedAt = now.AddDays(-10) },
            new() { Id = Guid.NewGuid(), UserId = userId5, Type = NotificationType.CommentMention, Message = "Nguyễn Duy Mạnh nhắc đến bạn trong comment: \"Thảo ơi, phần responsive mobile đang bị lỗi...\"", ReferenceId = taskId6, ReferenceType = ReferenceType.Task, IsRead = false, CreatedAt = now.AddDays(-3) },

            // Member added notifications
            new() { Id = Guid.NewGuid(), UserId = userId5, Type = NotificationType.MemberAdded, Message = "Bạn đã được thêm vào dự án \"Hệ thống Quản lý Dự án PCCV\" với vai trò Viewer.", ReferenceId = projectId1, ReferenceType = ReferenceType.Project, IsRead = true, CreatedAt = now.AddDays(-26) },
            new() { Id = Guid.NewGuid(), UserId = userId3, Type = NotificationType.MemberAdded, Message = "Bạn đã được thêm vào dự án \"Ứng dụng Mobile Banking\" với vai trò Member.", ReferenceId = projectId3, ReferenceType = ReferenceType.Project, IsRead = false, CreatedAt = now.AddDays(-12) }
        };

        context.Notifications.AddRange(notifications);

        // ── Save everything ──
        await context.SaveChangesAsync();
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }
}
