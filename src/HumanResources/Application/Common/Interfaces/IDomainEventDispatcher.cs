using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}