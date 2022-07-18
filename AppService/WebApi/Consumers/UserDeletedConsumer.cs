
using MassTransit;

using MediatR;

using YourBrand.IdentityService.Client;
using YourBrand.IdentityService.Contracts;
using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Users.Commands;

namespace YourBrand.Consumers;

public class AppServiceUserDeletedConsumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;
    private readonly IUsersClient _usersClient;
    private readonly ICurrentUserService _currentUserService;

    public AppServiceUserDeletedConsumer(IMediator mediator, IUsersClient usersClient, ICurrentUserService currentUserService)
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