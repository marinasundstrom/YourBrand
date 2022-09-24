using YourBrand.ApiKeys.Domain.Common;

namespace YourBrand.ApiKeys.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}