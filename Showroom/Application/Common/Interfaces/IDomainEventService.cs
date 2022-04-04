using YourBrand.Showroom.Domain.Common;

namespace YourBrand.Showroom.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}