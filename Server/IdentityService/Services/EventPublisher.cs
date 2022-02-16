using System;

using MassTransit;

using Microsoft.EntityFrameworkCore;

using Skynet.IdentityService.Application.Common.Interfaces;
using Skynet.IdentityService.Domain.Entities;

namespace Skynet.IdentityService.Services;

public class EventPublisher : IEventPublisher
{
    private readonly IBus _bus;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IBus bus, IServiceProvider serviceProvider, ILogger<EventPublisher> logger)
    {
        _bus = bus;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task PublishEvent(object ev)
    {
        _logger.LogInformation("Sending event");

        await _bus.Publish(ev);
    }
}