using YourBrand.Notifications.Domain.Common;

namespace YourBrand.Notifications.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}