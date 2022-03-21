using Worker.Application.Common.Interfaces;
using Worker.Domain.Entities;

namespace Worker.Application.Common.Interfaces;

public interface INotificationPublisher
{
    Task PublishNotification(Notification notification);
}