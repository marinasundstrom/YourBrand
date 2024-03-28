using YourBrand.Marketing.Domain;

namespace YourBrand.Marketing.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}