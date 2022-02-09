using Skynet.Domain.Common;

namespace Skynet.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}