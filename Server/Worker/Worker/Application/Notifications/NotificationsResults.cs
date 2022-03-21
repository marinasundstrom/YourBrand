using System;

using Worker.Application.Common.Models;

namespace Worker.Application.Notifications;

public record class NotificationsResults(IEnumerable<NotificationDto> Items, int? UnreadNotificationsCount, int TotalCount)
    : Results<NotificationDto>(Items, TotalCount);