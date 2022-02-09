using System;

using Skynet.Application.Common.Interfaces;
using Skynet.WebApi;

using Contracts;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

namespace Skynet.Consumers;

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