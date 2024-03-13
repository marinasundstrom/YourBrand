using YourBrand.Analytics.Domain;

namespace YourBrand.Analytics.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}