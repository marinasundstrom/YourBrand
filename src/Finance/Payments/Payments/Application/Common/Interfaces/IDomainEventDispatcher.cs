using YourBrand.Domain;
using YourBrand.Payments.Domain.Common;

namespace YourBrand.Payments.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}