using System;

using Contracts;

using MassTransit;

using MediatR;

using Worker.Application;

namespace Worker.Consumers;

public class DoSomethingConsumer : IConsumer<DoSomething>
{
    private readonly IMediator _mediator;

    public DoSomethingConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<DoSomething> context)
    {
        var message = context.Message;

        var result = await _mediator.Send(new DoSomethingCommand(message.LHS, message.RHS));

        await context.Publish(new DoSomethingResponse($"The result is: {result}"));
    }
}