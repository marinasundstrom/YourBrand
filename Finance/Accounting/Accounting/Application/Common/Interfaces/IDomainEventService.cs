using Accounting.Domain.Common;

namespace Accounting.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}