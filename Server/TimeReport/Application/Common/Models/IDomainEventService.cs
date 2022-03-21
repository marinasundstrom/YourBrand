using YourCompany.TimeReport.Domain.Common;

namespace YourCompany.TimeReport.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}