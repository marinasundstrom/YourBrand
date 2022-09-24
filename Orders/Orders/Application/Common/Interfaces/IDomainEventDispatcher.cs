using YourBrand.Orders.Domain.Common;

namespace YourBrand.Orders.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}