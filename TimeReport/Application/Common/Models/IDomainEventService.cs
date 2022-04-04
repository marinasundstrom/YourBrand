using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}