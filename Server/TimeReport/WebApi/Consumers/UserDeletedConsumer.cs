
using MassTransit;

using MediatR;

using Skynet.IdentityService.Client;
using Skynet.IdentityService.Contracts;
using Skynet.TimeReport.Application.Common.Interfaces;
using Skynet.TimeReport.Application.Users.Commands;

namespace Skynet.TimeReport.Consumers;

public class UserDeletedConsumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;
    private readonly IUsersClient _usersClient;

    public UserDeletedConsumer(IMediator mediator, IUsersClient usersClient)
    {
        _mediator = mediator;
        _usersClient = usersClient;
    }

    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        var result = await _mediator.Send(new DeleteUserCommand(message.UserId));
    }
}