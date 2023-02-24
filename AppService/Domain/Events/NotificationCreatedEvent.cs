using YourBrand.Domain.Common;

namespace YourBrand.Domain.Events;

public record NotificationCreatedEvent : DomainEvent
{
    public NotificationCreatedEvent(string notificationId)
    {
        this.NotificationId = notificationId;
    }

    public string NotificationId { get; }
}