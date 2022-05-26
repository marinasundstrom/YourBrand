using YourBrand.Transactions.Domain.Common;

namespace YourBrand.Transactions.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}