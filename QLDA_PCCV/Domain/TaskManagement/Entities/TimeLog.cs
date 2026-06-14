namespace QLDA_PCCV.Domain.TaskManagement.Entities;

public class TimeLog
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid UserId { get; set; }
    public decimal LoggedHours { get; set; }
    public DateOnly LogDate { get; set; }
    public string? Note { get; set; }
    public DateTime CreatedAt { get; set; }

    public TaskItem Task { get; set; } = null!;
}
