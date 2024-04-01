using MassTransit;

using YourBrand.IdentityManagement.Application.Common.Interfaces;

namespace YourBrand.IdentityManagement.Services;

public class EventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<EventPublisher> _logger;

    public EventPublisher(IPublishEndpoint publishEndpoint, IServiceProvider serviceProvider, ILogger<EventPublisher> logger)
    {
        _publishEndpoint = publishEndpoint;
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task PublishEvent<T>(T ev)
        where T : class
    {
        _logger.LogInformation("Sending event");

        //var endpoint = await _bus.GetPublishSendEndpoint<T>();

        //await endpoint.Send(ev);

        await _publishEndpoint.Publish(ev);

    }
}