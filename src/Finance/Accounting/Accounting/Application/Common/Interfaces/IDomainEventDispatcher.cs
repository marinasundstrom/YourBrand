using YourBrand.Accounting.Domain.Common;
using YourBrand.Domain;

namespace YourBrand.Accounting.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}