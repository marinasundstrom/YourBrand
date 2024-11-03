using YourBrand.Domain;
using YourBrand.Notifications.Domain.Common;

namespace YourBrand.Notifications.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}