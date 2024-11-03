using YourBrand.ApiKeys.Domain.Common;
using YourBrand.Domain;

namespace YourBrand.ApiKeys.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}