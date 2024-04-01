namespace YourBrand.Domain.Infrastructure;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}