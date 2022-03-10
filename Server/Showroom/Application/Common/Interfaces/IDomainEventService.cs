using Skynet.Showroom.Domain.Common;

namespace Skynet.Showroom.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}