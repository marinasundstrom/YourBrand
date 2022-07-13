using YourBrand.Warehouse.Domain.Common;

namespace YourBrand.Warehouse.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}