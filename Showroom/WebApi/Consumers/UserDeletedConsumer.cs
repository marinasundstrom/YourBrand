
using MassTransit;

using MediatR;

using YourBrand.IdentityService.Client;
using YourBrand.IdentityService.Contracts;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Users.Commands;

namespace YourBrand.Showroom.Consumers;

public class ShowroomUserDeletedConsumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public ShowroomUserDeletedConsumer(IMediator mediator, ICurrentUserService currentUserService)
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