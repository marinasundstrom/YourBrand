using MassTransit;

using MediatR;

using YourBrand.UserManagement.Application.Users.Commands;
using YourBrand.UserManagement.Contracts;
using YourBrand.Identity;

namespace YourBrand.UserManagement.Consumers;

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

        var user = await _mediator.Send(new CreateUserCommand(message.OrganizationId, message.FirstName, message.LastName, message.DisplayName, message.Role, message.Email));

        await context.RespondAsync(new CreateUserResponse(user.Id, user.Organization.Id, user.FirstName, user.LastName, user.DisplayName, user.Email));
    }
}
