using Invoices.Domain.Common;

namespace Invoices.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}