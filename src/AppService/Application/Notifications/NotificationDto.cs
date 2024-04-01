namespace YourBrand.Application.Notifications;

public record NotificationDto(
    string Id, string Title, string? Text, string? Link, DateTime Published, bool IsRead);