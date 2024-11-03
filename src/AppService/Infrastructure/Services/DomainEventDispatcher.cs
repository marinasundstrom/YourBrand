﻿using MediatR;

using Microsoft.Extensions.Logging;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain;
using YourBrand.Domain.Common;

namespace YourBrand.Infrastructure.Services;

sealed class DomainEventDispatcher(ILogger<DomainEventDispatcher> logger, IPublisher mediator) : IDomainEventDispatcher
{
    public async Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
        await mediator.Publish(domainEvent, cancellationToken);
    }
}