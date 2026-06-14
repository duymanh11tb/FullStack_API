using QLDA_PCCV.Domain.Notifications.Enums;

namespace QLDA_PCCV.Domain.Notifications.Entities;

public class Notification
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public NotificationType Type { get; set; }
    public string Message { get; set; } = string.Empty;
    public Guid ReferenceId { get; set; }
    public ReferenceType ReferenceType { get; set; }
    public bool IsRead { get; set; }
    public DateTime CreatedAt { get; set; }

    public User User { get; set; } = null!;
}
