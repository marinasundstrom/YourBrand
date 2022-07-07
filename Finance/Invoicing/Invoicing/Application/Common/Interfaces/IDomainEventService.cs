using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}