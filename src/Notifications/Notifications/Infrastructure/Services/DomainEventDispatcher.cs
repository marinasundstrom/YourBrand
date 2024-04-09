using MediatR;

using YourBrand.Notifications.Application.Common.Interfaces;
using YourBrand.Notifications.Domain.Common;

namespace YourBrand.Notifications.Infrastructure.Services;

class DomainEventDispatcher(ILogger<DomainEventDispatcher> logger, IPublisher mediator) : IDomainEventDispatcher
{
    public async Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
        await mediator.Publish(domainEvent, cancellationToken);
    }
}