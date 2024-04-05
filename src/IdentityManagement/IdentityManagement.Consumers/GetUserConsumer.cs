using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.IdentityManagement.Consumers;

public class GetUserConsumer : IConsumer<GetUser>
{
    private readonly IMediator _mediator;
    private readonly IUserContext _userContext;

    public GetUserConsumer(IMediator mediator, IUserContext userContext)
    {
        _mediator = mediator;
        _userContext = userContext;
    }

    public async Task Consume(ConsumeContext<GetUser> context)
    {
        var message = context.Message;

        var user = await _mediator.Send(new YourBrand.IdentityManagement.Application.Users.Queries.GetUserQuery(message.UserId));

        await context.RespondAsync(new GetUserResponse(user.Id, user.Tenant.Id, null!, user.FirstName, user.LastName, user.DisplayName, user.Email));
    }
}