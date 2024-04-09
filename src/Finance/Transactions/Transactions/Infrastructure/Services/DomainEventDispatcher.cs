using MediatR;

using YourBrand.Transactions.Application.Common.Interfaces;
using YourBrand.Transactions.Domain.Common;

namespace YourBrand.Transactions.Infrastructure.Services;

class DomainEventDispatcher(ILogger<DomainEventDispatcher> logger, IPublisher mediator) : IDomainEventDispatcher
{
    public async Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
        await mediator.Publish(domainEvent, cancellationToken);
    }
}