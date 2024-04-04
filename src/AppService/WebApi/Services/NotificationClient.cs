using Microsoft.AspNetCore.SignalR;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Notifications.Client;
using YourBrand.WebApi.Hubs;

namespace YourBrand.WebApi.Services;

public class NotificationClient : INotificationClient
{
    private readonly IHubContext<NotificationHub, INotificationClient> _notificationsHubContext;

    public NotificationClient(IHubContext<NotificationHub, INotificationClient> notificationsHubContext)
    {
        _notificationsHubContext = notificationsHubContext;
    }

    public async Task NotificationReceived(Notification notification)
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