using YourBrand.Warehouse.Domain.Common;

namespace YourBrand.Warehouse.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}