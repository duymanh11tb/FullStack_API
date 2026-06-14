using QLDA_PCCV.Domain.ProjectManagement.Enums;

namespace QLDA_PCCV.Domain.ProjectManagement.Entities;

public class Project
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = "#2563EB";
    public ProjectStatus Status { get; set; } = ProjectStatus.Active;
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public Guid OwnerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();
    public ICollection<Sprint> Sprints { get; set; } = new List<Sprint>();
    public ICollection<Milestone> Milestones { get; set; } = new List<Milestone>();
}
