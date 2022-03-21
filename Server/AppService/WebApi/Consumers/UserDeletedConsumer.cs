
using MassTransit;

using MediatR;

using YourCompany.IdentityService.Client;
using YourCompany.IdentityService.Contracts;
using YourCompany.Application.Common.Interfaces;
using YourCompany.Application.Users.Commands;

namespace YourCompany.Consumers;

public class UserDeleted0Consumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;
    private readonly IUsersClient _usersClient;
    private readonly ICurrentUserService _currentUserService;

    public UserDeleted0Consumer(IMediator mediator, IUsersClient usersClient, ICurrentUserService currentUserService)
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