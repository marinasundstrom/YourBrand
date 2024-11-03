using MediatR;

using Microsoft.Extensions.Logging;

using YourBrand.Domain;
using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Infrastructure.Services;

sealed class DomainEventDispatcher(ILogger<DomainEventDispatcher> logger, IPublisher mediator) : IDomainEventDispatcher
{
    public async Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
        await mediator.Publish(domainEvent, cancellationToken);
    }
}