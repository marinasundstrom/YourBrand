using YourBrand.Messenger.Domain.Common;

namespace YourBrand.Messenger.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}