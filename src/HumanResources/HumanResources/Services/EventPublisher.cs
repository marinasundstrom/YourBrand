using MassTransit;

using YourBrand.HumanResources.Application.Common.Interfaces;

namespace YourBrand.HumanResources.Services;

public class EventPublisher(IBus bus, IServiceProvider serviceProvider, ILogger<EventPublisher> logger) : IEventPublisher
{
    public async Task PublishEvent<T>(T ev)
        where T : class
    {
        logger.LogInformation("Sending event");

        //var endpoint = await _bus.GetPublishSendEndpoint<T>();

        //await endpoint.Send(ev);

        await bus.Publish<T>(ev);

    }
}