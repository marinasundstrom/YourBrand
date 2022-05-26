using YourBrand.Accounting.Domain.Common;

namespace YourBrand.Accounting.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}