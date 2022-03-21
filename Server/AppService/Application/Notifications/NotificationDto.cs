using System;

namespace YourCompany.Application.Notifications;

public record NotificationDto(
    string Id, string Title, string? Text, string? Link, DateTime Published, bool IsRead);