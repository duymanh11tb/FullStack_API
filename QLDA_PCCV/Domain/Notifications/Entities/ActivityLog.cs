using QLDA_PCCV.Domain.Notifications.Enums;

namespace QLDA_PCCV.Domain.Notifications.Entities;

public class ActivityLog
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid UserId { get; set; }
    public ActivityAction Action { get; set; }
    public string? Detail { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
