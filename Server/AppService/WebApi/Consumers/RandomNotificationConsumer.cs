using Catalog.Application.Common.Interfaces;
using Catalog.WebApi.Hubs;

using Contracts;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

namespace Catalog.Consumers;

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