using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}