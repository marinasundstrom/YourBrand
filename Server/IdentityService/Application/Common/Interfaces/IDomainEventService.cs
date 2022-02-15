using Skynet.IdentityService.Domain.Common;

namespace Skynet.IdentityService.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}