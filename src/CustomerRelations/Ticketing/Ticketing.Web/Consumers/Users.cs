using MassTransit;

using MediatR;

using YourBrand.Ticketing.Application.Features.Users;
using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.Ticketing.Consumers;

public class TicketingUserCreatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ILogger<TicketingUserCreatedConsumer> logger) : IConsumer<UserCreated>
{
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        try
        {
            var message = context.Message;

            var messageR = await requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId));
            var message2 = messageR.Message;

            var result = await mediator.Send(new YourBrand.Ticketing.Application.Features.Users.CreateUser($"{message2.FirstName} {message2.LastName}", message2.Email, message.TenantId, message2.UserId));
        }
        catch (Exception e)
        {
            logger.LogError(e, "FOO");
        }
    }
}

public class TicketingUserDeletedConsumer(IMediator mediator) : IConsumer<UserDeleted>
{
    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        await mediator.Send(new DeleteUser(message.UserId));
    }
}


public class TicketingUserUpdatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient) : IConsumer<UserUpdated>
{
    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var message = context.Message;

        var messageR = await requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId));
        var message2 = messageR.Message;

        var result = await mediator.Send(new UpdateUser(message2.UserId, $"{message2.FirstName} {message2.LastName}", message2.Email));
    }
}