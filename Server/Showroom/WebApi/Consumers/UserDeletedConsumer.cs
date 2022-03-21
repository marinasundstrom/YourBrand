
using MassTransit;

using MediatR;

using YourCompany.IdentityService.Client;
using YourCompany.IdentityService.Contracts;
using YourCompany.Showroom.Application.Common.Interfaces;
using YourCompany.Showroom.Application.Users.Commands;

namespace YourCompany.Showroom.Consumers;

public class UserDeleted1Consumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public UserDeleted1Consumer(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.DeletedById);

        var result = await _mediator.Send(new DeleteUserCommand(message.UserId));
    }
}