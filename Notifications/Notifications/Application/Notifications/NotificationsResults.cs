using System;

using YourBrand.Notifications.Application.Common.Models;

namespace YourBrand.Notifications.Application.Notifications;

public record class NotificationsResults(IEnumerable<NotificationDto> Items, int? UnreadNotificationsCount, int TotalCount)
    : Results<NotificationDto>(Items, TotalCount);