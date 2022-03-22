using Worker.Client;

namespace YourBrand.Application.Common.Interfaces;

public interface INotificationClient
{
    Task NotificationReceived(NotificationDto notification);
}