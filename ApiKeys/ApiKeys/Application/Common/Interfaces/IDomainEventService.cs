using YourBrand.ApiKeys.Domain.Common;

namespace YourBrand.ApiKeys.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}