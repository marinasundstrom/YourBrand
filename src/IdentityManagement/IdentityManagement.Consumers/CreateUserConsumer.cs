using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Users.Commands;
using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.IdentityManagement.Consumers;

public class CreateUserConsumer : IConsumer<CreateUser>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public CreateUserConsumer(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<CreateUser> context)
    {
        var message = context.Message;

        var user = await _mediator.Send(new CreateUserCommand(message.TenantId, message.FirstName, message.LastName, message.DisplayName, message.Role, message.Email));

        await context.RespondAsync(new CreateUserResponse(user.Id, user.Tenant.Id, null!, user.FirstName, user.LastName, user.DisplayName, user.Email));
    }
}