
using YourBrand.IdentityService.Contracts;
using YourBrand.ApiKeys.Application.Common.Interfaces;
using YourBrand.ApiKeys.Application.Users.Commands;

using MassTransit;

using MediatR;

namespace YourBrand.ApiKeys.Consumers;

public class UserDeleted5Consumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public UserDeleted5Consumer(IMediator mediator, ICurrentUserService currentUserService)
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