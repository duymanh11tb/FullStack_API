using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace QLDA_PCCV.Services
{
    public interface IExternalServiceClient
    {
        Task<TaskItemDto?> GetTaskAsync(Guid taskId, CancellationToken cancellationToken = default);
        Task<List<ProjectMemberDto>?> GetProjectMembersAsync(Guid projectId, CancellationToken cancellationToken = default);
        Task<bool> IsUserInProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
        Task<int?> GetUserRoleInProjectAsync(Guid projectId, Guid userId, CancellationToken cancellationToken = default);
    }

    public class TaskItemDto
    {
        public Guid TaskId { get; set; }
        public Guid BoardId { get; set; } // maps to ProjectId
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid? AssigneeId { get; set; }
    }

    public class ProjectMemberDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Role { get; set; } // 0: Owner, 1: Manager, 2: Member, 3: Viewer
    }
}
