namespace YourBrand.Application.Notifications;

public record NotificationDto(
    string Id, string Content, string? Link, DateTime Published, bool IsRead);