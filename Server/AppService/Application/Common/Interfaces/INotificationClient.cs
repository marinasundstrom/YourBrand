using Worker.Client;

namespace Skynet.Application.Common.Interfaces;

public interface INotificationClient
{
    Task NotificationReceived(NotificationDto notification);
}