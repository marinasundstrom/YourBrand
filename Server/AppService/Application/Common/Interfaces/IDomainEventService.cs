using Catalog.Domain.Common;

namespace Catalog.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}