using Transactions.Domain.Common;

namespace Transactions.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}