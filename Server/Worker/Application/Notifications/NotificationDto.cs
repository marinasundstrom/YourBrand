namespace Worker.Application.Notifications;

public record NotificationDto(
    string Id, string Title, string? Text, string? Tag, string? Link, string? UserId, bool IsRead, DateTime? Read, DateTime? Published, DateTime? ScheduledFor,
    DateTime Created, string CreatedBy, DateTime? LastModified, string? LastModifiedBy);