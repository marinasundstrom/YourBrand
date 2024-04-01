using Contracts;

using MassTransit;

using YourBrand.Application.Common.Interfaces;

namespace YourBrand.Consumers;

public class DoSomethingResponseConsumer : IConsumer<DoSomethingResponse>
{
    private readonly ISomethingClient _somethingClient;

    public DoSomethingResponseConsumer(ISomethingClient somethingClient)
    {
        _somethingClient = somethingClient;
    }

    public async Task Consume(ConsumeContext<DoSomethingResponse> context)
    {
        var message = context.Message;

        await _somethingClient.ResponseReceived(message.Message);
    }
}