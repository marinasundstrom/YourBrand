namespace Contracts;

public record NotificationDto(
    string Id, DateTime? Published, string Title, string? Text, string? Link, string? UserId, bool IsRead,
    DateTimeOffset Created, string CreatedBy, DateTimeOffset? LastModified, string? LastModifiedBy);