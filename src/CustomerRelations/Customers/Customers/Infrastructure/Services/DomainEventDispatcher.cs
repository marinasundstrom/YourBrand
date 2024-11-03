﻿using MediatR;

using YourBrand.Customers.Application.Common.Interfaces;
using YourBrand.Customers.Domain.Common;
using YourBrand.Domain;

namespace YourBrand.Customers.Infrastructure.Services;

sealed class DomainEventDispatcher(ILogger<DomainEventDispatcher> logger, IPublisher mediator) : IDomainEventDispatcher
{
    public async Task Dispatch(DomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Publishing domain event. Event - {event}", domainEvent.GetType().Name);
        await mediator.Publish(domainEvent, cancellationToken);
    }
}