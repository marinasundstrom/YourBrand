using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}