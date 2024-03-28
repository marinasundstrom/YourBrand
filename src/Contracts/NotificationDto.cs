namespace Contracts;

public record NotificationDto(
    string Id, DateTime? Published, string Title, string? Text, string? Link, string? UserId, bool IsRead,
    DateTime Created, string CreatedBy, DateTime? LastModified, string? LastModifiedBy);