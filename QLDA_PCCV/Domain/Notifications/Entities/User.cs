using QLDA_PCCV.Domain.Notifications.Enums;

namespace QLDA_PCCV.Domain.Notifications.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public UserRole Role { get; set; } = UserRole.User;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<ActivityLog> ActivityLogs { get; set; } = new List<ActivityLog>();
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public NotificationSetting? NotificationSetting { get; set; }
}
