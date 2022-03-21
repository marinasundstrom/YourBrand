
using MassTransit;

using MediatR;

using YourCompany.IdentityService.Client;
using YourCompany.IdentityService.Contracts;
using YourCompany.TimeReport.Application.Common.Interfaces;
using YourCompany.TimeReport.Application.Users.Commands;

namespace YourCompany.TimeReport.Consumers;

public class UserDeletedConsumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;
    private readonly IUsersClient _usersClient;
    private readonly ICurrentUserService _currentUserService;

    public UserDeletedConsumer(IMediator mediator, IUsersClient usersClient, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _usersClient = usersClient;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.DeletedById);

        var result = await _mediator.Send(new DeleteUserCommand(message.UserId));
    }
}