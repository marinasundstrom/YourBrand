using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.Showroom.Application.Users.Commands;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Consumers;

public class ShowroomUserCreatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ILogger<ShowroomUserCreatedConsumer> logger) : IConsumer<UserCreated>
{
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var message = context.Message;

        var messageR = await requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId));
        var message2 = messageR.Message;

        var result = await mediator.Send(new CreateUserCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, "SSN", message2.Email));
    }
}

public class ShowroomUserDeletedConsumer(IMediator mediator) : IConsumer<UserDeleted>
{
    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        await mediator.Send(new DeleteUserCommand(message.UserId));
    }
}

public class ShowroomUserUpdatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient) : IConsumer<UserUpdated>
{
    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var message = context.Message;

        var messageR = await requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId));
        var message2 = messageR.Message;

        var result = await mediator.Send(new UpdateUserCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, "SSN", message2.Email));
    }
}