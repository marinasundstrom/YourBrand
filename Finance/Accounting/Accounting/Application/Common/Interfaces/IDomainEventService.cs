using YourBrand.Accounting.Domain.Common;

namespace YourBrand.Accounting.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}