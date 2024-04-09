using MassTransit;

using YourBrand.IdentityManagement.Application.Common.Interfaces;

namespace YourBrand.IdentityManagement.Services;

public class EventPublisher(IPublishEndpoint publishEndpoint, IServiceProvider serviceProvider, ILogger<EventPublisher> logger) : IEventPublisher
{
    public async Task PublishEvent<T>(T ev)
        where T : class
    {
        logger.LogInformation("Sending event");

        //var endpoint = await _bus.GetPublishSendEndpoint<T>();

        //await endpoint.Send(ev);

        await publishEndpoint.Publish(ev);

    }
}