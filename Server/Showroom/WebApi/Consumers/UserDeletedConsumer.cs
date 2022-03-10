
using MassTransit;

using MediatR;

using Skynet.IdentityService.Client;
using Skynet.IdentityService.Contracts;
using Skynet.Showroom.Application.Common.Interfaces;
using Skynet.Showroom.Application.Users.Commands;

namespace Skynet.Showroom.Consumers;

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