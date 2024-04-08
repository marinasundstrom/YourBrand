namespace YourBrand.Notifications.Contracts;

public record SendNotification
{
    public required string TenantId { get; set; }
    public required string Content { get; set; }
    public string? Link { get; set; }
    public string? UserId { get; set; }
    public DateTimeOffset? ScheduledFor { get; set; }
    public required string CreatedById { get; set; }
}

public record SendNotificationResponse;

public record MarkAllNotificationsAsRead
{
    public required string TenantId { get; set; }
}

public record MarkAllNotificationsAsReadResponse;

public record MarkNotificationAsRead
{
    public required string TenantId { get; set; }
    public required string NotificationId { get; set; }
}

public record MarkNotificationAsReadResponse;