using YourBrand.Domain;
using YourBrand.Showroom.Domain.Common;

namespace YourBrand.Showroom.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}