using YourBrand.UserManagement.Domain.Common;

namespace YourBrand.UserManagement.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}