using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.Messenger.Application.Users.Commands;

namespace YourBrand.Messenger.Consumers;

public class MessengerUserCreatedConsumer : IConsumer<UserCreated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetUser> _requestClient;
    private readonly ILogger<MessengerUserCreatedConsumer> _logger;

    public MessengerUserCreatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ILogger<MessengerUserCreatedConsumer> logger)
    {
        _mediator = mediator;
        _requestClient = requestClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        try
        {
            var message = context.Message;

            var messageR = await _requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId, (message.CreatedById)));
            var message2 = messageR.Message;

            var result = await _mediator.Send(new CreateUserCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, message2.Email));
        }
        catch (Exception e)
        {
            _logger.LogError(e, "FOO");
        }
    }
}

public class MessengerUserDeletedConsumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;

    public MessengerUserDeletedConsumer(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        await _mediator.Send(new DeleteUserCommand(message.UserId));
    }
}

public class MessengerUserUpdatedConsumer : IConsumer<UserUpdated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetUser> _requestClient;

    public MessengerUserUpdatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient)
    {
        _mediator = mediator;
        _requestClient = requestClient;
    }

    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var message = context.Message;

        var messageR = await _requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId, message.UpdatedById));
        var message2 = messageR.Message;

        var result = await _mediator.Send(new UpdateUserCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, message2.Email));
    }
}