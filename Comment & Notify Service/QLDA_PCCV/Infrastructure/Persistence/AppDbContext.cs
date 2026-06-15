using Microsoft.EntityFrameworkCore;
using QLDA_PCCV.Domain.Notifications.Entities;
using QLDA_PCCV.Domain.Notifications.Enums;

namespace QLDA_PCCV.Infrastructure.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<ActivityLog> ActivityLogs => Set<ActivityLog>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<NotificationSetting> NotificationSettings => Set<NotificationSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        ConfigureNotifications(modelBuilder);
    }

    private static void ConfigureNotifications(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.Email).HasMaxLength(200).IsRequired();
            entity.Property(x => x.Username).HasMaxLength(100).IsRequired();
            entity.Property(x => x.PasswordHash).HasMaxLength(500).IsRequired();
            entity.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            entity.Property(x => x.AvatarUrl).HasMaxLength(500);
            entity.Property(x => x.Role).HasConversion<string>().HasMaxLength(20).HasDefaultValue(UserRole.User).IsRequired();
            entity.Property(x => x.IsActive).HasDefaultValue(true);
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone");

            entity.HasIndex(x => x.Email).IsUnique();
            entity.HasIndex(x => x.Username).IsUnique();
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.Token).HasMaxLength(500).IsRequired();
            entity.Property(x => x.ExpiresAt).HasColumnType("timestamp with time zone");
            entity.Property(x => x.IsRevoked).HasDefaultValue(false);
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(x => x.User)
                .WithMany(x => x.RefreshTokens)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => x.Token).IsUnique();
            entity.HasIndex(x => new { x.UserId, x.IsRevoked });
        });

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.ToTable("Comments");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.Content).HasColumnType("text").IsRequired();
            entity.Property(x => x.IsDeleted).HasDefaultValue(false);
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(x => x.UpdatedAt).HasColumnType("timestamp with time zone");

            entity.HasOne(x => x.User)
                .WithMany(x => x.Comments)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.ParentComment)
                .WithMany(x => x.Replies)
                .HasForeignKey(x => x.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.TaskId);
            entity.HasIndex(x => x.UserId);
            entity.HasIndex(x => x.ParentCommentId);
        });

        modelBuilder.Entity<ActivityLog>(entity =>
        {
            entity.ToTable("ActivityLogs");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.Action)
                .HasConversion(action => ToDatabaseValue(action), value => ToActivityAction(value))
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(x => x.Detail).HasColumnType("text");
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(x => x.User)
                .WithMany(x => x.ActivityLogs)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(x => x.TaskId);
            entity.HasIndex(x => x.ProjectId);
            entity.HasIndex(x => new { x.UserId, x.CreatedAt });
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.ToTable("Notifications");
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Id).HasDefaultValueSql("gen_random_uuid()");
            entity.Property(x => x.Type)
                .HasConversion(type => ToDatabaseValue(type), value => ToNotificationType(value))
                .HasMaxLength(50)
                .IsRequired();
            entity.Property(x => x.Message).HasMaxLength(500).IsRequired();
            entity.Property(x => x.ReferenceType).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(x => x.IsRead).HasDefaultValue(false);
            entity.Property(x => x.CreatedAt).HasColumnType("timestamp with time zone").HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(x => new { x.UserId, x.IsRead, x.CreatedAt });
            entity.HasIndex(x => new { x.ReferenceType, x.ReferenceId });
        });

        modelBuilder.Entity<NotificationSetting>(entity =>
        {
            entity.ToTable("NotificationSettings");
            entity.HasKey(x => x.UserId);
            entity.Property(x => x.OnComment).HasDefaultValue(true);
            entity.Property(x => x.OnAssigned).HasDefaultValue(true);
            entity.Property(x => x.OnStatusChanged).HasDefaultValue(true);
            entity.Property(x => x.OnMention).HasDefaultValue(true);
            entity.Property(x => x.OnSprintStarted).HasDefaultValue(true);
            entity.Property(x => x.OnMemberAdded).HasDefaultValue(true);

            entity.HasOne(x => x.User)
                .WithOne(x => x.NotificationSetting)
                .HasForeignKey<NotificationSetting>(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static string ToDatabaseValue(ActivityAction action) => action switch
    {
        ActivityAction.Commented => "commented",
        ActivityAction.StatusChanged => "status_changed",
        ActivityAction.Assigned => "assigned",
        ActivityAction.MemberAdded => "member_added",
        _ => throw new ArgumentOutOfRangeException(nameof(action), action, null)
    };

    private static ActivityAction ToActivityAction(string value) => value switch
    {
        "commented" => ActivityAction.Commented,
        "status_changed" => ActivityAction.StatusChanged,
        "assigned" => ActivityAction.Assigned,
        "member_added" => ActivityAction.MemberAdded,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    private static string ToDatabaseValue(NotificationType type) => type switch
    {
        NotificationType.TaskAssigned => "task.assigned",
        NotificationType.TaskStatusChanged => "task.status.changed",
        NotificationType.CommentMention => "comment.mention",
        NotificationType.SprintStarted => "sprint.started",
        NotificationType.MemberAdded => "member.added",
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
    };

    private static NotificationType ToNotificationType(string value) => value switch
    {
        "task.assigned" => NotificationType.TaskAssigned,
        "task.status.changed" => NotificationType.TaskStatusChanged,
        "comment.mention" => NotificationType.CommentMention,
        "sprint.started" => NotificationType.SprintStarted,
        "member.added" => NotificationType.MemberAdded,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };
}
