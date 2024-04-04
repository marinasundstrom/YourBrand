namespace YourBrand.Notifications.Contracts;

public record NotificationDto(
    string Id, DateTimeOffset? Published, string Content, string? Link, string? UserId, bool IsRead,
    DateTimeOffset Created, string CreatedBy, DateTimeOffset? LastModified, string? LastModifiedById);