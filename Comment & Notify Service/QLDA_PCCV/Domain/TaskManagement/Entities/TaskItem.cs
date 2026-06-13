using QLDA_PCCV.Domain.TaskManagement.Enums;

namespace QLDA_PCCV.Domain.TaskManagement.Entities;

public class TaskItem
{
    public Guid Id { get; set; }
    public Guid ProjectId { get; set; }
    public Guid? SprintId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskItemStatus Status { get; set; } = TaskItemStatus.Backlog;
    public TaskPriority Priority { get; set; } = TaskPriority.Medium;
    public Guid? AssigneeId { get; set; }
    public Guid ReporterId { get; set; }
    public int? StoryPoints { get; set; }
    public DateOnly? Deadline { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<SubTask> SubTasks { get; set; } = new List<SubTask>();
    public ICollection<TimeLog> TimeLogs { get; set; } = new List<TimeLog>();
    public ICollection<TaskLabel> Labels { get; set; } = new List<TaskLabel>();
}
