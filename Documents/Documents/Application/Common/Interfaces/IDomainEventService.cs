using YourBrand.Documents.Domain.Common;

namespace YourBrand.Documents.Application.Common.Interfaces;

public interface IDomainEventService
{
    Task Publish(DomainEvent domainEvent);
}