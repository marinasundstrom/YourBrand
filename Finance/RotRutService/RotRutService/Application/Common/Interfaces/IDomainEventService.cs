using YourBrand.RotRutService.Domain.Common;

namespace YourBrand.RotRutService.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}