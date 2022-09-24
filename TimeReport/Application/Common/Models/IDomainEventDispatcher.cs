using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default);
}