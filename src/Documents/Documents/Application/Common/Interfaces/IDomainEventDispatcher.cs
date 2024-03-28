using YourBrand.Documents.Domain.Common;

namespace YourBrand.Documents.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}