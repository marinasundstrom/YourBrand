using YourBrand.Transactions.Domain.Common;

namespace YourBrand.Transactions.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}