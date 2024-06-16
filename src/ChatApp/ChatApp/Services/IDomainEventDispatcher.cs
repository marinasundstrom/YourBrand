using YourBrand.Domain;

namespace YourBrand.ChatApp.Services;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}