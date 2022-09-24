using YourBrand.IdentityService.Domain.Common;

namespace YourBrand.IdentityService.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}