using Contracts;

using MassTransit;

using YourBrand.Application.Common.Interfaces;

namespace YourBrand.Consumers;

public class RandomNotificationConsumer(IWorkerClient workerClient) : IConsumer<RandomNotification>
{
    public async Task Consume(ConsumeContext<RandomNotification> context)
    {
        var message = context.Message;

        await workerClient.NotificationReceived(message.Message);
    }
}