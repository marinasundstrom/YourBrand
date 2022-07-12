using YourBrand.Marketing.Domain.Common;

namespace YourBrand.Marketing.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}