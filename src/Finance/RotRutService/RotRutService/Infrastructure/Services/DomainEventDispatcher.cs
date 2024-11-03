using MediatR;

using YourBrand.Domain;
using YourBrand.RotRutService.Application.Common.Interfaces;
using YourBrand.RotRutService.Domain.Common;

namespace YourBrand.RotRutService.Infrastructure.Services;

sealed class DomainEventDispatcher(ILogger<DomainEventDispatcher> logger, IPublisher mediator) : IDomainEventDispatcher
{
    public async Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
        await mediator.Publish(domainEvent, cancellationToken);
    }
}