using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Users.Commands;
using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.IdentityManagement.Consumers;

public class CreateUserConsumer(IMediator mediator) : IConsumer<CreateUser>
{
    public async Task Consume(ConsumeContext<CreateUser> context)
    {
        var message = context.Message;

        var user = await mediator.Send(new CreateUserCommand(message.OrganizationId, message.FirstName, message.LastName, message.DisplayName, message.Role, message.Email, message.Password));

        await context.RespondAsync(new CreateUserResponse(user.Id, user.Tenant.Id, user.FirstName, user.LastName, user.DisplayName, user.Email));
    }
}