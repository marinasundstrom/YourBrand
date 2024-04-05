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
    private readonly IUserContext _userContext;

    public MessengerUserCreatedConsumer(IMediator mediator, IUserContext userContext, IRequestClient<GetUser> requestClient, ILogger<MessengerUserCreatedConsumer> logger)
    {
        _mediator = mediator;
        _userContext = userContext;
        _requestClient = requestClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        try
        {
            var message = context.Message;

            _userContext.SetCurrentUser(message.CreatedById);

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
    private readonly IUserContext _userContext;

    public MessengerUserDeletedConsumer(IMediator mediator, IUserContext userContext)
    {
        _mediator = mediator;
        _userContext = userContext;
    }

    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        _userContext.SetCurrentUser(message.DeletedById);

        await _mediator.Send(new DeleteUserCommand(message.UserId));
    }
}

public class MessengerUserUpdatedConsumer : IConsumer<UserUpdated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetUser> _requestClient;
    private readonly IUserContext _userContext;

    public MessengerUserUpdatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, IUserContext userContext)
    {
        _mediator = mediator;
        _requestClient = requestClient;
        _userContext = userContext;
    }

    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var message = context.Message;

        _userContext.SetCurrentUser(message.UpdatedById);

        var messageR = await _requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId, message.UpdatedById));
        var message2 = messageR.Message;

        var result = await _mediator.Send(new UpdateUserCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, message2.Email));
    }
}