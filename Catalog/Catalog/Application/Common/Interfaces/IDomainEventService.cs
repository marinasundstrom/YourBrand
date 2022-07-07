using YourBrand.Catalog.Domain.Common;

namespace YourBrand.Catalog.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}