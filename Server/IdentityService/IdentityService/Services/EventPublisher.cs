using System;

using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourCompany.IdentityService.Application.Common.Interfaces;
using YourCompany.IdentityService.Domain.Entities;

namespace YourCompany.IdentityService.Services;

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

    public async Task PublishEvent<T>(T ev)
        where T : class
    {
        _logger.LogInformation("Sending event");

        //var endpoint = await _bus.GetPublishSendEndpoint<T>();

        //await endpoint.Send(ev);

        await _bus.Publish<T>(ev);

    }
}