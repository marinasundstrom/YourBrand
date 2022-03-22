using YourBrand.Domain.Common;

namespace YourBrand.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}