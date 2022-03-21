using YourCompany.Domain.Common;

namespace YourCompany.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}