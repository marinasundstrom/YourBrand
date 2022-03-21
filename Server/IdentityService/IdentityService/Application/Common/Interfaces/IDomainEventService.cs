using YourCompany.IdentityService.Domain.Common;

namespace YourCompany.IdentityService.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}