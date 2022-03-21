using YourCompany.Domain.Common;

namespace YourCompany.Domain.Events;

public class NotificationCreatedEvent : DomainEvent
{
    public NotificationCreatedEvent(string notificationId)
    {
        this.NotificationId = notificationId;
    }

    public string NotificationId { get; }
}