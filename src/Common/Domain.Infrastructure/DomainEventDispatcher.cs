using MediatR;

using Microsoft.Extensions.Logging;

namespace YourBrand.Domain.Infrastructure;

sealed class DomainEventDispatcher(ILogger<DomainEventDispatcher> logger, IPublisher mediator) : IDomainEventDispatcher
{
    public async Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken)
    {
        logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
        await mediator.Publish(domainEvent);
    }
}