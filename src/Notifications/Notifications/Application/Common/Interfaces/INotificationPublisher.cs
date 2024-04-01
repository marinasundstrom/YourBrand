using YourBrand.Notifications.Domain.Entities;

namespace YourBrand.Notifications.Application.Common.Interfaces;

public interface INotificationPublisher
{
    Task PublishNotification(Notification notification);
}