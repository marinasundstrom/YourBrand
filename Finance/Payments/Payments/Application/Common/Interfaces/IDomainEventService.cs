using YourBrand.Payments.Domain.Common;

namespace YourBrand.Payments.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}