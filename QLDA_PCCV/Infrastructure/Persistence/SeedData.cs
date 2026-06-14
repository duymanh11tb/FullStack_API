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
        // 2. PROJECTS (3 projects)
        // ══════════════════════════════════════════════════════════════
        var projects = new List<Project>
        {
            new()
            {
                Id = projectId1,
                Name = "Hệ thống Quản lý Dự án PCCV",
                Description = "Xây dựng hệ thống quản lý dự án phân công công việc cho doanh nghiệp vừa và nhỏ. Hệ thống bao gồm quản lý task, sprint, comment và thông báo realtime.",
                Color = "#2563EB",
                Status = ProjectStatus.Active,
                StartDate = DateOnly.FromDateTime(now.AddDays(-30)),
                EndDate = DateOnly.FromDateTime(now.AddDays(60)),
                OwnerId = userId1,
                CreatedAt = now.AddDays(-30)
            },
            new()
            {
                Id = projectId2,
                Name = "Website Thương mại Điện tử",
                Description = "Phát triển nền tảng TMĐT với các tính năng: quản lý sản phẩm, giỏ hàng, thanh toán online, quản lý đơn hàng và báo cáo doanh thu.",
                Color = "#10B981",
                Status = ProjectStatus.Active,
                StartDate = DateOnly.FromDateTime(now.AddDays(-20)),
                EndDate = DateOnly.FromDateTime(now.AddDays(90)),
                OwnerId = userId2,
                CreatedAt = now.AddDays(-20)
            },
            new()
            {
                Id = projectId3,
                Name = "Ứng dụng Mobile Banking",
                Description = "Thiết kế và phát triển ứng dụng mobile banking cho ngân hàng ABC, hỗ trợ chuyển khoản, thanh toán hóa đơn, quản lý tài khoản và tích hợp eKYC.",
                Color = "#F59E0B",
                Status = ProjectStatus.Active,
                StartDate = DateOnly.FromDateTime(now.AddDays(-15)),
                EndDate = DateOnly.FromDateTime(now.AddDays(120)),
                OwnerId = userId1,
                CreatedAt = now.AddDays(-15)
            }
        };

        context.Projects.AddRange(projects);

        // ══════════════════════════════════════════════════════════════
        // 3. PROJECT MEMBERS
        // ══════════════════════════════════════════════════════════════
        var members = new List<ProjectMember>
        {
            // Project 1 – QLDA
            new() { Id = Guid.NewGuid(), ProjectId = projectId1, UserId = userId1, Role = ProjectMemberRole.Owner, JoinedAt = now.AddDays(-30) },
            new() { Id = Guid.NewGuid(), ProjectId = projectId1, UserId = userId2, Role = ProjectMemberRole.Manager, JoinedAt = now.AddDays(-29) },
            new() { Id = Guid.NewGuid(), ProjectId = projectId1, UserId = userId3, Role = ProjectMemberRole.Member, JoinedAt = now.AddDays(-28) },
            new() { Id = Guid.NewGuid(), ProjectId = projectId1, UserId = userId4, Role = ProjectMemberRole.Member, JoinedAt = now.AddDays(-27) },
            new() { Id = Guid.NewGuid(), ProjectId = projectId1, UserId = userId5, Role = ProjectMemberRole.Viewer, JoinedAt = now.AddDays(-26) },

            // Project 2 – TMĐT
            new() { Id = Guid.NewGuid(), ProjectId = projectId2, UserId = userId2, Role = ProjectMemberRole.Owner, JoinedAt = now.AddDays(-20) },
            new() { Id = Guid.NewGuid(), ProjectId = projectId2, UserId = userId3, Role = ProjectMemberRole.Manager, JoinedAt = now.AddDays(-19) },
            new() { Id = Guid.NewGuid(), ProjectId = projectId2, UserId = userId5, Role = ProjectMemberRole.Member, JoinedAt = now.AddDays(-18) },

            // Project 3 – Mobile Banking
            new() { Id = Guid.NewGuid(), ProjectId = projectId3, UserId = userId1, Role = ProjectMemberRole.Owner, JoinedAt = now.AddDays(-15) },
            new() { Id = Guid.NewGuid(), ProjectId = projectId3, UserId = userId4, Role = ProjectMemberRole.Manager, JoinedAt = now.AddDays(-14) },
            new() { Id = Guid.NewGuid(), ProjectId = projectId3, UserId = userId2, Role = ProjectMemberRole.Member, JoinedAt = now.AddDays(-13) },
            new() { Id = Guid.NewGuid(), ProjectId = projectId3, UserId = userId3, Role = ProjectMemberRole.Member, JoinedAt = now.AddDays(-12) }
        };

        context.ProjectMembers.AddRange(members);

        // ══════════════════════════════════════════════════════════════
        // 4. SPRINTS
        // ══════════════════════════════════════════════════════════════
        var sprints = new List<Sprint>
        {
            // Project 1 sprints
            new()
            {
                Id = sprintId1,
                ProjectId = projectId1,
                Name = "Sprint 1 – Thiết kế cơ sở dữ liệu & API",
                Goal = "Hoàn thành thiết kế database schema, viết các API CRUD cơ bản cho User, Project và Task.",
                Status = SprintStatus.Completed,
                StartDate = DateOnly.FromDateTime(now.AddDays(-30)),
                EndDate = DateOnly.FromDateTime(now.AddDays(-16)),
                CreatedAt = now.AddDays(-30)
            },
            new()
            {
                Id = sprintId2,
                ProjectId = projectId1,
                Name = "Sprint 2 – Comment & Notification",
                Goal = "Xây dựng hệ thống comment nested, notification realtime và activity logging.",
                Status = SprintStatus.Active,
                StartDate = DateOnly.FromDateTime(now.AddDays(-15)),
                EndDate = DateOnly.FromDateTime(now.AddDays(-1)),
                CreatedAt = now.AddDays(-15)
            },
            // Project 2 sprints
            new()
            {
                Id = sprintId3,
                ProjectId = projectId2,
                Name = "Sprint 1 – Quản lý sản phẩm",
                Goal = "Xây dựng module quản lý sản phẩm: thêm/sửa/xóa sản phẩm, upload ảnh, phân loại danh mục.",
                Status = SprintStatus.Active,
                StartDate = DateOnly.FromDateTime(now.AddDays(-20)),
                EndDate = DateOnly.FromDateTime(now.AddDays(-6)),
                CreatedAt = now.AddDays(-20)
            },
            // Project 3 sprints
            new()
            {
                Id = sprintId4,
                ProjectId = projectId3,
                Name = "Sprint 1 – Xác thực & eKYC",
                Goal = "Triển khai luồng đăng ký, đăng nhập bằng OTP, tích hợp eKYC với FPT AI và xác thực CCCD.",
                Status = SprintStatus.Planning,
                StartDate = DateOnly.FromDateTime(now.AddDays(-10)),
                EndDate = DateOnly.FromDateTime(now.AddDays(4)),
                CreatedAt = now.AddDays(-10)
            }
        };

        context.Sprints.AddRange(sprints);

        // ══════════════════════════════════════════════════════════════
        // 5. MILESTONES
        // ══════════════════════════════════════════════════════════════
        var milestones = new List<Milestone>
        {
            new()
            {
                Id = milestoneId1,
                ProjectId = projectId1,
                Title = "MVP – Bản demo đầu tiên",
                DueDate = DateOnly.FromDateTime(now.AddDays(-5)),
                IsCompleted = true,
                CreatedAt = now.AddDays(-30)
            },
            new()
            {
                Id = milestoneId2,
                ProjectId = projectId1,
                Title = "Release v1.0 – Bản chính thức",
                DueDate = DateOnly.FromDateTime(now.AddDays(30)),
                IsCompleted = false,
                CreatedAt = now.AddDays(-25)
            },
            new()
            {
                Id = milestoneId3,
                ProjectId = projectId2,
                Title = "UAT – Kiểm thử người dùng",
                DueDate = DateOnly.FromDateTime(now.AddDays(45)),
                IsCompleted = false,
                CreatedAt = now.AddDays(-18)
            },
            new()
            {
                Id = milestoneId4,
                ProjectId = projectId3,
                Title = "Hoàn thành tích hợp eKYC",
                DueDate = DateOnly.FromDateTime(now.AddDays(20)),
                IsCompleted = false,
                CreatedAt = now.AddDays(-10)
            }
        };

        context.Milestones.AddRange(milestones);

        // ══════════════════════════════════════════════════════════════
        // 6. TASKS (10 tasks across projects)
        // ══════════════════════════════════════════════════════════════
        var tasks = new List<TaskItem>
        {
            // ── Project 1 tasks ──
            new()
            {
                Id = taskId1,
                ProjectId = projectId1,
                SprintId = sprintId1,
                Title = "Thiết kế ERD Database",
                Description = "Thiết kế sơ đồ Entity Relationship Diagram cho toàn bộ hệ thống bao gồm: Users, Projects, Tasks, Comments, Notifications. Sử dụng SQL Server làm DBMS chính.",
                Status = TaskItemStatus.Done,
                Priority = TaskPriority.High,
                AssigneeId = userId2,
                ReporterId = userId1,
                StoryPoints = 5,
                Deadline = DateOnly.FromDateTime(now.AddDays(-20)),
                CreatedAt = now.AddDays(-29),
                UpdatedAt = now.AddDays(-18)
            },
            new()
            {
                Id = taskId2,
                ProjectId = projectId1,
                SprintId = sprintId1,
                Title = "Viết API CRUD cho Project",
                Description = "Implement các endpoint REST API cho quản lý Project: GET /api/projects, POST /api/projects, PUT /api/projects/{id}, DELETE /api/projects/{id}. Bao gồm validation và error handling.",
                Status = TaskItemStatus.Done,
                Priority = TaskPriority.High,
                AssigneeId = userId3,
                ReporterId = userId1,
                StoryPoints = 8,
                Deadline = DateOnly.FromDateTime(now.AddDays(-17)),
                CreatedAt = now.AddDays(-28),
                UpdatedAt = now.AddDays(-16)
            },
            new()
            {
                Id = taskId3,
                ProjectId = projectId1,
                SprintId = sprintId2,
                Title = "Xây dựng hệ thống Comment nested",
                Description = "Phát triển tính năng comment với khả năng reply nhiều cấp (nested comments). Hỗ trợ soft delete, edit comment và hiển thị thread comment theo dạng cây.",
                Status = TaskItemStatus.InProgress,
                Priority = TaskPriority.Critical,
                AssigneeId = userId2,
                ReporterId = userId1,
                StoryPoints = 13,
                Deadline = DateOnly.FromDateTime(now.AddDays(-3)),
                CreatedAt = now.AddDays(-14),
                UpdatedAt = now.AddDays(-2)
            },
            new()
            {
                Id = taskId4,
                ProjectId = projectId1,
                SprintId = sprintId2,
                Title = "Tích hợp Notification Service",
                Description = "Xây dựng hệ thống thông báo realtime. Khi có comment mới, task được assign, hoặc status thay đổi → tự động gửi notification đến user liên quan.",
                Status = TaskItemStatus.ToDo,
                Priority = TaskPriority.High,
                AssigneeId = userId4,
                ReporterId = userId2,
                StoryPoints = 8,
                Deadline = DateOnly.FromDateTime(now.AddDays(5)),
                CreatedAt = now.AddDays(-12)
            },
            new()
            {
                Id = taskId5,
                ProjectId = projectId1,
                SprintId = sprintId2,
                Title = "Viết Unit Test cho Comment API",
                Description = "Viết unit test và integration test cho Comment Controller. Đạt tối thiểu 80% code coverage. Test các trường hợp: CRUD, nested reply, soft delete, authorization.",
                Status = TaskItemStatus.Backlog,
                Priority = TaskPriority.Medium,
                AssigneeId = userId3,
                ReporterId = userId2,
                StoryPoints = 5,
                Deadline = DateOnly.FromDateTime(now.AddDays(10)),
                CreatedAt = now.AddDays(-10)
            },

            // ── Project 2 tasks ──
            new()
            {
                Id = taskId6,
                ProjectId = projectId2,
                SprintId = sprintId3,
                Title = "Thiết kế UI trang danh sách sản phẩm",
                Description = "Thiết kế giao diện trang danh sách sản phẩm với filter theo danh mục, tìm kiếm, sắp xếp theo giá/tên. Sử dụng Vue.js component và responsive design.",
                Status = TaskItemStatus.InProgress,
                Priority = TaskPriority.High,
                AssigneeId = userId5,
                ReporterId = userId2,
                StoryPoints = 8,
                Deadline = DateOnly.FromDateTime(now.AddDays(-2)),
                CreatedAt = now.AddDays(-18),
                UpdatedAt = now.AddDays(-3)
            },
            new()
            {
                Id = taskId7,
                ProjectId = projectId2,
                SprintId = sprintId3,
                Title = "Xây dựng API quản lý giỏ hàng",
                Description = "Develop Shopping Cart API: thêm/xóa sản phẩm, cập nhật số lượng, tính tổng tiền, áp dụng mã giảm giá. Lưu trữ cart trong database cho user đã đăng nhập.",
                Status = TaskItemStatus.Review,
                Priority = TaskPriority.Critical,
                AssigneeId = userId3,
                ReporterId = userId2,
                StoryPoints = 13,
                Deadline = DateOnly.FromDateTime(now.AddDays(-1)),
                CreatedAt = now.AddDays(-16),
                UpdatedAt = now.AddDays(-1)
            },
            new()
            {
                Id = taskId8,
                ProjectId = projectId2,
                SprintId = sprintId3,
                Title = "Tích hợp cổng thanh toán VNPay",
                Description = "Tích hợp VNPay Payment Gateway cho thanh toán online. Hỗ trợ các phương thức: thẻ ATM nội địa, Visa/Mastercard, QR Code và ví điện tử.",
                Status = TaskItemStatus.ToDo,
                Priority = TaskPriority.High,
                AssigneeId = userId5,
                ReporterId = userId3,
                StoryPoints = 13,
                Deadline = DateOnly.FromDateTime(now.AddDays(15)),
                CreatedAt = now.AddDays(-14)
            },

            // ── Project 3 tasks ──
            new()
            {
                Id = taskId9,
                ProjectId = projectId3,
                SprintId = sprintId4,
                Title = "Thiết kế luồng đăng ký OTP",
                Description = "Thiết kế và implement luồng đăng ký tài khoản qua SMS OTP: nhập SĐT → gửi OTP → xác thực → tạo mật khẩu → hoàn tất. Tích hợp Twilio hoặc Firebase Auth.",
                Status = TaskItemStatus.InProgress,
                Priority = TaskPriority.Critical,
                AssigneeId = userId4,
                ReporterId = userId1,
                StoryPoints = 8,
                Deadline = DateOnly.FromDateTime(now.AddDays(3)),
                CreatedAt = now.AddDays(-8),
                UpdatedAt = now.AddDays(-1)
            },
            new()
            {
                Id = taskId10,
                ProjectId = projectId3,
                SprintId = sprintId4,
                Title = "Tích hợp eKYC FPT AI",
                Description = "Tích hợp API FPT AI để thực hiện eKYC: chụp CCCD mặt trước/sau, nhận diện khuôn mặt, so khớp thông tin và trả về kết quả xác thực danh tính.",
                Status = TaskItemStatus.Backlog,
                Priority = TaskPriority.High,
                AssigneeId = userId2,
                ReporterId = userId1,
                StoryPoints = 21,
                Deadline = DateOnly.FromDateTime(now.AddDays(18)),
                CreatedAt = now.AddDays(-7)
            }
        };

        context.Tasks.AddRange(tasks);

        // ══════════════════════════════════════════════════════════════
        // 7. SUBTASKS
        // ══════════════════════════════════════════════════════════════
        var subTasks = new List<SubTask>
        {
            // Subtasks for Task 1 (ERD)
            new() { Id = Guid.NewGuid(), TaskId = taskId1, Title = "Phân tích yêu cầu nghiệp vụ", IsCompleted = true, AssigneeId = userId2, CreatedAt = now.AddDays(-29) },
            new() { Id = Guid.NewGuid(), TaskId = taskId1, Title = "Vẽ sơ đồ ERD trên draw.io", IsCompleted = true, AssigneeId = userId2, CreatedAt = now.AddDays(-28) },
            new() { Id = Guid.NewGuid(), TaskId = taskId1, Title = "Review ERD với team", IsCompleted = true, AssigneeId = userId1, CreatedAt = now.AddDays(-27) },

            // Subtasks for Task 3 (Comment nested)
            new() { Id = Guid.NewGuid(), TaskId = taskId3, Title = "Thiết kế schema bảng Comments", IsCompleted = true, AssigneeId = userId2, CreatedAt = now.AddDays(-14) },
            new() { Id = Guid.NewGuid(), TaskId = taskId3, Title = "Viết API POST /api/comments", IsCompleted = true, AssigneeId = userId2, CreatedAt = now.AddDays(-13) },
            new() { Id = Guid.NewGuid(), TaskId = taskId3, Title = "Implement nested reply logic", IsCompleted = false, AssigneeId = userId2, CreatedAt = now.AddDays(-12) },
            new() { Id = Guid.NewGuid(), TaskId = taskId3, Title = "Viết API soft delete comment", IsCompleted = false, AssigneeId = userId2, CreatedAt = now.AddDays(-11) },

            // Subtasks for Task 6 (UI sản phẩm)
            new() { Id = Guid.NewGuid(), TaskId = taskId6, Title = "Tạo component ProductCard", IsCompleted = true, AssigneeId = userId5, CreatedAt = now.AddDays(-17) },
            new() { Id = Guid.NewGuid(), TaskId = taskId6, Title = "Implement filter & search", IsCompleted = false, AssigneeId = userId5, CreatedAt = now.AddDays(-16) },
            new() { Id = Guid.NewGuid(), TaskId = taskId6, Title = "Responsive cho mobile", IsCompleted = false, AssigneeId = userId5, CreatedAt = now.AddDays(-15) },

            // Subtasks for Task 9 (OTP)
            new() { Id = Guid.NewGuid(), TaskId = taskId9, Title = "Setup Twilio SDK", IsCompleted = true, AssigneeId = userId4, CreatedAt = now.AddDays(-7) },
            new() { Id = Guid.NewGuid(), TaskId = taskId9, Title = "Viết API gửi OTP", IsCompleted = true, AssigneeId = userId4, CreatedAt = now.AddDays(-6) },
            new() { Id = Guid.NewGuid(), TaskId = taskId9, Title = "Viết API xác thực OTP", IsCompleted = false, AssigneeId = userId4, CreatedAt = now.AddDays(-5) }
        };

        context.SubTasks.AddRange(subTasks);

        // ══════════════════════════════════════════════════════════════
        // 8. TASK LABELS
        // ══════════════════════════════════════════════════════════════
        var labels = new List<TaskLabel>
        {
            new() { Id = Guid.NewGuid(), TaskId = taskId1, LabelName = "database", Color = "#8B5CF6" },
            new() { Id = Guid.NewGuid(), TaskId = taskId1, LabelName = "design", Color = "#EC4899" },
            new() { Id = Guid.NewGuid(), TaskId = taskId2, LabelName = "backend", Color = "#3B82F6" },
            new() { Id = Guid.NewGuid(), TaskId = taskId2, LabelName = "api", Color = "#10B981" },
            new() { Id = Guid.NewGuid(), TaskId = taskId3, LabelName = "backend", Color = "#3B82F6" },
            new() { Id = Guid.NewGuid(), TaskId = taskId3, LabelName = "feature", Color = "#F59E0B" },
            new() { Id = Guid.NewGuid(), TaskId = taskId4, LabelName = "realtime", Color = "#EF4444" },
            new() { Id = Guid.NewGuid(), TaskId = taskId5, LabelName = "testing", Color = "#6B7280" },
            new() { Id = Guid.NewGuid(), TaskId = taskId6, LabelName = "frontend", Color = "#06B6D4" },
            new() { Id = Guid.NewGuid(), TaskId = taskId6, LabelName = "ui/ux", Color = "#EC4899" },
            new() { Id = Guid.NewGuid(), TaskId = taskId7, LabelName = "backend", Color = "#3B82F6" },
            new() { Id = Guid.NewGuid(), TaskId = taskId7, LabelName = "api", Color = "#10B981" },
            new() { Id = Guid.NewGuid(), TaskId = taskId8, LabelName = "integration", Color = "#F97316" },
            new() { Id = Guid.NewGuid(), TaskId = taskId8, LabelName = "payment", Color = "#EF4444" },
            new() { Id = Guid.NewGuid(), TaskId = taskId9, LabelName = "mobile", Color = "#8B5CF6" },
            new() { Id = Guid.NewGuid(), TaskId = taskId9, LabelName = "security", Color = "#DC2626" },
            new() { Id = Guid.NewGuid(), TaskId = taskId10, LabelName = "integration", Color = "#F97316" },
            new() { Id = Guid.NewGuid(), TaskId = taskId10, LabelName = "ai", Color = "#7C3AED" }
        };

        context.TaskLabels.AddRange(labels);

        // ══════════════════════════════════════════════════════════════
        // 9. TIME LOGS
        // ══════════════════════════════════════════════════════════════
        var timeLogs = new List<TimeLog>
        {
            new() { Id = Guid.NewGuid(), TaskId = taskId1, UserId = userId2, LoggedHours = 3.5m, LogDate = DateOnly.FromDateTime(now.AddDays(-25)), Note = "Phân tích yêu cầu, vẽ ERD v1", CreatedAt = now.AddDays(-25) },
            new() { Id = Guid.NewGuid(), TaskId = taskId1, UserId = userId2, LoggedHours = 4.0m, LogDate = DateOnly.FromDateTime(now.AddDays(-24)), Note = "Chỉnh sửa ERD sau review", CreatedAt = now.AddDays(-24) },
            new() { Id = Guid.NewGuid(), TaskId = taskId2, UserId = userId3, LoggedHours = 6.0m, LogDate = DateOnly.FromDateTime(now.AddDays(-22)), Note = "Implement CRUD Project API", CreatedAt = now.AddDays(-22) },
            new() { Id = Guid.NewGuid(), TaskId = taskId2, UserId = userId3, LoggedHours = 2.5m, LogDate = DateOnly.FromDateTime(now.AddDays(-21)), Note = "Thêm validation và error handling", CreatedAt = now.AddDays(-21) },
            new() { Id = Guid.NewGuid(), TaskId = taskId3, UserId = userId2, LoggedHours = 5.0m, LogDate = DateOnly.FromDateTime(now.AddDays(-10)), Note = "Thiết kế schema và viết API comment", CreatedAt = now.AddDays(-10) },
            new() { Id = Guid.NewGuid(), TaskId = taskId3, UserId = userId2, LoggedHours = 4.0m, LogDate = DateOnly.FromDateTime(now.AddDays(-8)), Note = "Implement nested reply logic", CreatedAt = now.AddDays(-8) },
            new() { Id = Guid.NewGuid(), TaskId = taskId6, UserId = userId5, LoggedHours = 7.0m, LogDate = DateOnly.FromDateTime(now.AddDays(-12)), Note = "Tạo ProductCard component và layout", CreatedAt = now.AddDays(-12) },
            new() { Id = Guid.NewGuid(), TaskId = taskId7, UserId = userId3, LoggedHours = 6.5m, LogDate = DateOnly.FromDateTime(now.AddDays(-9)), Note = "Viết Shopping Cart API", CreatedAt = now.AddDays(-9) },
            new() { Id = Guid.NewGuid(), TaskId = taskId9, UserId = userId4, LoggedHours = 3.0m, LogDate = DateOnly.FromDateTime(now.AddDays(-6)), Note = "Setup Twilio và viết API gửi OTP", CreatedAt = now.AddDays(-6) },
            new() { Id = Guid.NewGuid(), TaskId = taskId9, UserId = userId4, LoggedHours = 4.5m, LogDate = DateOnly.FromDateTime(now.AddDays(-4)), Note = "Implement luồng xác thực OTP", CreatedAt = now.AddDays(-4) }
        };

        context.TimeLogs.AddRange(timeLogs);

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
