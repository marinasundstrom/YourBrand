using System;

using Catalog.Application.Common.Interfaces;
using Catalog.WebApi.Hubs;

using Microsoft.AspNetCore.SignalR;

using Worker.Client;

namespace Catalog.WebApi.Services;

public class NotificationClient : INotificationClient
{
    private readonly IHubContext<NotificationHub, INotificationClient> _notificationsHubContext;

    public NotificationClient(IHubContext<NotificationHub, INotificationClient> notificationsHubContext)
    {
        _notificationsHubContext = notificationsHubContext;
    }

    public async Task NotificationReceived(NotificationDto notification)
    {
        if (notification.UserId is not null)
        {
            await _notificationsHubContext.Clients.User(notification.UserId).NotificationReceived(notification);
        }
        else
        {
            await _notificationsHubContext.Clients.All.NotificationReceived(notification);
        }
    }
}