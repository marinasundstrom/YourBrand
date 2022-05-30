using YourBrand.Customers.Domain.Common;

namespace YourBrand.Customers.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}