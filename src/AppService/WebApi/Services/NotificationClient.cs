using Microsoft.AspNetCore.SignalR;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Notifications.Client;
using YourBrand.WebApi.Hubs;

namespace YourBrand.WebApi.Services;

public class NotificationClient(IHubContext<NotificationHub, INotificationClient> notificationsHubContext) : INotificationClient
{
    public async Task NotificationReceived(Notification notification)
    {
        if (notification.UserId is not null)
        {
            await notificationsHubContext.Clients.User(notification.UserId).NotificationReceived(notification);
        }
        else
        {
            await notificationsHubContext.Clients.All.NotificationReceived(notification);
        }
    }
}