using YourBrand.IdentityManagement.Domain.Common;

namespace YourBrand.IdentityManagement.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}