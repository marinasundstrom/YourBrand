namespace YourBrand.Notifications.Application.Notifications;

public record NotificationDto(
    string Id, string? Content, string? Tag, string? Link, string? UserId, bool IsRead, DateTimeOffset? Read, DateTimeOffset? Published, DateTimeOffset? ScheduledFor,
    DateTimeOffset Created, string CreatedBy, DateTimeOffset? LastModified, string? LastModifiedBy);