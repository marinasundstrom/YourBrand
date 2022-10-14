using YourBrand.Inventory.Domain.Common;

namespace YourBrand.Inventory.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}