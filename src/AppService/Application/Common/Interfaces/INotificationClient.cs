using YourBrand.Notifications.Client;

namespace YourBrand.Application.Common.Interfaces;

public interface INotificationClient
{
    Task NotificationReceived(Notification notification);
}