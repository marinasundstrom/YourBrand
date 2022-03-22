using YourBrand.IdentityService.Domain.Common;

namespace YourBrand.IdentityService.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}