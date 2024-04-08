using MassTransit;

using MediatR;

using YourBrand.IdentityManagement.Contracts;
using YourBrand.Sales.Features.OrderManagement.Users;

namespace YourBrand.Sales.Consumers;

public class SalesUserCreatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ILogger<SalesUserCreatedConsumer> logger) : IConsumer<UserCreated>
{
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        try
        {
            var message = context.Message;

            var messageR = await requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId, (message.CreatedById)));
            var message2 = messageR.Message;

            var result = await mediator.Send(new Sales.Features.OrderManagement.Users.CreateUser($"{message2.FirstName} {message2.LastName}", message2.Email, message.TenantId, message2.UserId));
        }
        catch (Exception e)
        {
            logger.LogError(e, "FOO");
        }
    }
}

public class SalesUserDeletedConsumer(IMediator mediator) : IConsumer<UserDeleted>
{
    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        await mediator.Send(new DeleteUser(message.UserId));
    }
}


public class SalesUserUpdatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient) : IConsumer<UserUpdated>
{
    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var message = context.Message;

        var messageR = await requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId, message.UpdatedById));
        var message2 = messageR.Message;

        var result = await mediator.Send(new UpdateUser(message2.UserId, $"{message2.FirstName} {message2.LastName}", message2.Email));
    }
}