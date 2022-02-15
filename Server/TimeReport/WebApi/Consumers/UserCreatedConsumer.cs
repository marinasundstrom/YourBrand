using System;

using Contracts;

using MassTransit;

using MediatR;

using Skynet.TimeReport.Application;
using Skynet.TimeReport.Application.Users.Commands;

namespace Skynet.TimeReport.Consumers;

public class UserCreatedConsumer : IConsumer<UserCreated>
{
    private readonly IMediator _mediator;

    public UserCreatedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var message = context.Message;

        var result = await _mediator.Send(new UpdateUserCommand(message.UserId, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty));
    }
}

public class UserUpdatedConsumer : IConsumer<UserUpdated>
{
    private readonly IMediator _mediator;

    public UserUpdatedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var message = context.Message;

    }
}

public class UserDeletedConsumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;

    public UserDeletedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        var result = await _mediator.Send(new DeleteUserCommand(message.UserId));
    }
}