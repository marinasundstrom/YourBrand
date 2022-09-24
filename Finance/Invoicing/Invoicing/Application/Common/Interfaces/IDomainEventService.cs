using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}