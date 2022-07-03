using YourBrand.Products.Domain.Common;

namespace YourBrand.Products.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}