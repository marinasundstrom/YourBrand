using YourBrand.Customers.Domain.Common;
using YourBrand.Domain;

namespace YourBrand.Customers.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}