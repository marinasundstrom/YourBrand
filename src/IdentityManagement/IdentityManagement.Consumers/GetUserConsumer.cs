using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.IdentityManagement.Consumers;

public class GetUserConsumer(IMediator mediator) : IConsumer<GetUser>
{
    public async Task Consume(ConsumeContext<GetUser> context)
    {
        var message = context.Message;

        var user = await mediator.Send(new YourBrand.IdentityManagement.Application.Users.Queries.GetUserQuery(message.UserId));

        await context.RespondAsync(new GetUserResponse(user.Id, user.Tenant.Id, null!, user.FirstName, user.LastName, user.DisplayName, user.Email));
    }
}