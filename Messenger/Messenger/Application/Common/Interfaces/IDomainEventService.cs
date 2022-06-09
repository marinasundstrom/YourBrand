using YourBrand.Messenger.Domain.Common;

namespace YourBrand.Messenger.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}