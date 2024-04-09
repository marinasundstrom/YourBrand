using MassTransit;

using MediatR;

using YourBrand.Application.Users.Commands;
using YourBrand.Identity;
using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.Consumers;

public class AppServiceUserCreatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ILogger<AppServiceUserCreatedConsumer> logger) : IConsumer<UserCreated>
{
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        try
        {
            var message = context.Message;

            var messageR = await requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId));
            var message2 = messageR.Message;

            var result = await mediator.Send(new CreateUserCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, "SSN", message2.Email));
        }
        catch (Exception e)
        {
            logger.LogError(e, "FOO");
        }
    }
}

public class AppServiceUserDeletedConsumer(IMediator mediator) : IConsumer<UserDeleted>
{
    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        await mediator.Send(new DeleteUserCommand(message.UserId));
    }
}

public class AppServiceUserUpdatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient) : IConsumer<UserUpdated>
{
    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var message = context.Message;

        var messageR = await requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId));
        var message2 = messageR.Message;

        var result = await mediator.Send(new UpdateUserCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, "SSN", message2.Email));
    }
}