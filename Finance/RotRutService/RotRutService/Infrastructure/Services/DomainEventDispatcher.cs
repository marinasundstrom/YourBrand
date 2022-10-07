using YourBrand.RotRutService.Application.Common.Interfaces;
using YourBrand.RotRutService.Application.Common.Models;
using YourBrand.RotRutService.Domain.Common;

using MediatR;

using Microsoft.Extensions.Logging;

namespace YourBrand.RotRutService.Infrastructure.Services;

class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly ILogger<DomainEventDispatcher> _logger;
    private readonly IPublisher _mediator;

    public DomainEventDispatcher(ILogger<DomainEventDispatcher> logger, IPublisher mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }

    public async Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
        await _mediator.Publish(domainEvent, cancellationToken);
    }
}