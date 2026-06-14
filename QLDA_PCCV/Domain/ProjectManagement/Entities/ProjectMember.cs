using QLDA_PCCV.Domain.ProjectManagement.Enums;

namespace QLDA_PCCV.Domain.ProjectManagement.Entities;

public class ProjectMember
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid UserId { get; set; }
    public ProjectMemberRole Role { get; set; } = ProjectMemberRole.Member;
    public DateTime JoinedAt { get; set; }

    public Project Project { get; set; } = null!;
}
