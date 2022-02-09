using TimeReport.Domain.Common;

namespace TimeReport.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}