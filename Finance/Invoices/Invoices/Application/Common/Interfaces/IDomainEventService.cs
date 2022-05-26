using YourBrand.Invoices.Domain.Common;

namespace YourBrand.Invoices.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}