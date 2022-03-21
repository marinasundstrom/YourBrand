using YourCompany.Application.Common.Interfaces;

using Contracts;

using MassTransit;

namespace YourCompany.Consumers;

public class RandomNotificationConsumer : IConsumer<RandomNotification>
{
    private readonly IWorkerClient _workerClient;

    public RandomNotificationConsumer(IWorkerClient workerClient)
    {
        _workerClient = workerClient;
    }

    public async Task Consume(ConsumeContext<RandomNotification> context)
    {
        var message = context.Message;

        await _workerClient.NotificationReceived(message.Message);
    }
}