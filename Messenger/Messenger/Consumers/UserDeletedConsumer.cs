
using YourBrand.IdentityService.Contracts;
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Application.Users.Commands;

using MassTransit;

using MediatR;

namespace YourBrand.Messenger.Consumers;

public class MessengerUserDeletedConsumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public MessengerUserDeletedConsumer(IMediator mediator, ICurrentUserService currentUserService)
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