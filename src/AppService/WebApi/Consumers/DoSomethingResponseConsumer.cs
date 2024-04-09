using Contracts;

using MassTransit;

using YourBrand.Application.Common.Interfaces;

namespace YourBrand.Consumers;

public class DoSomethingResponseConsumer(ISomethingClient somethingClient) : IConsumer<DoSomethingResponse>
{
    public async Task Consume(ConsumeContext<DoSomethingResponse> context)
    {
        var message = context.Message;

        await somethingClient.ResponseReceived(message.Message);
    }
}