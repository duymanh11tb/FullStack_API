namespace QLDA_PCCV.Domain.TaskManagement.Entities;

public class TaskLabel
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string LabelName { get; set; } = string.Empty;
    public string Color { get; set; } = "#6B7280";

    public TaskItem Task { get; set; } = null!;
}
