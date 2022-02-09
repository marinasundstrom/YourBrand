using Skynet.TimeReport.Domain.Common;

namespace Skynet.TimeReport.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}