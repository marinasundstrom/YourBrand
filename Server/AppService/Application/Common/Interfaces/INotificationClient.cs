using Worker.Client;

namespace YourCompany.Application.Common.Interfaces;

public interface INotificationClient
{
    Task NotificationReceived(NotificationDto notification);
}