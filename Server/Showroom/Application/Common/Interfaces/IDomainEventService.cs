using YourCompany.Showroom.Domain.Common;

namespace YourCompany.Showroom.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}