using YourBrand.Domain;
using YourBrand.Notifications.Domain.Common;

namespace YourBrand.Notifications.Domain.Events;

public record NotificationCreatedEvent : DomainEvent
{
    public NotificationCreatedEvent(string notificationId)
    {
        this.NotificationId = notificationId;
    }

    public string NotificationId { get; }
}