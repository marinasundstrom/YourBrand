using System;

using YourBrand.Application.Common.Interfaces;
using YourBrand.WebApi;

using Contracts;

using MassTransit;

using Microsoft.AspNetCore.SignalR;

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