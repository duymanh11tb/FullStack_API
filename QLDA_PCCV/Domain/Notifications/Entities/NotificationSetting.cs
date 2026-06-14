namespace QLDA_PCCV.Domain.Notifications.Entities;

public class NotificationSetting
{
    public Guid UserId { get; set; }
    public bool OnComment { get; set; } = true;
    public bool OnAssigned { get; set; } = true;
    public bool OnStatusChanged { get; set; } = true;
    public bool OnMention { get; set; } = true;
    public bool OnSprintStarted { get; set; } = true;
    public bool OnMemberAdded { get; set; } = true;

    public User User { get; set; } = null!;
}
