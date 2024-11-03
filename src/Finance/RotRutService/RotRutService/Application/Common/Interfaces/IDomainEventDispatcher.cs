using YourBrand.Domain;
using YourBrand.RotRutService.Domain.Common;

namespace YourBrand.RotRutService.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}