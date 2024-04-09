
using MediatR;

using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Domain.Common;

namespace YourBrand.Messenger.Infrastructure.Services;

sealed class DomainEventDispatcher(ILogger<DomainEventDispatcher> logger, IPublisher mediator) : IDomainEventDispatcher
{
    public async Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
        await mediator.Publish(domainEvent, cancellationToken);
    }
}