using QLDA_PCCV.Domain.ProjectManagement.Enums;

namespace QLDA_PCCV.Domain.ProjectManagement.Entities;

public class Sprint
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Goal { get; set; }
    public SprintStatus Status { get; set; } = SprintStatus.Planning;
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }
    public DateTime CreatedAt { get; set; }

    public Project Project { get; set; } = null!;
}
