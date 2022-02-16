
using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Skynet.IdentityService.Client;
using Skynet.IdentityService.Contracts;
using Skynet.TimeReport.Application.Common.Interfaces;
using Skynet.TimeReport.Application.Users.Commands;

namespace Skynet.TimeReport.Consumers;

public class UserUpdatedConsumer : IConsumer<UserUpdated>
{
    private readonly IMediator _mediator;
    private readonly IUsersClient _usersClient;

    public UserUpdatedConsumer(IMediator mediator, IUsersClient usersClient)
    {
        _mediator = mediator;
        _usersClient = usersClient;
    }

    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var message = context.Message;

        var userResponse = await _usersClient.GetUserAsync(message.UserId);

        var result = await _mediator.Send(new UpdateUserCommand(userResponse.Id, userResponse.FirstName, userResponse.LastName, userResponse.DisplayName, userResponse.Ssn, userResponse.Email));

    }
}
