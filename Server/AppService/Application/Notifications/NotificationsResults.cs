using System;

using Skynet.Application.Common.Models;

namespace Skynet.Application.Notifications;

public record class NotificationsResults(IEnumerable<NotificationDto> Items, int? UnreadNotificationsCount, int TotalCount)
    : Results<NotificationDto>(Items, TotalCount);