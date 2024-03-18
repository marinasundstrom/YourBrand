using YourBrand.UserManagement.Contracts;
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Application.Users.Commands;

using MassTransit;

using MediatR;
using YourBrand.Identity;

namespace YourBrand.Messenger.Consumers;

public class MessengerUserCreatedConsumer : IConsumer<UserCreated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetUser> _requestClient;
    private readonly ILogger<MessengerUserCreatedConsumer> _logger;
    private readonly ICurrentUserService _currentUserService;

    public MessengerUserCreatedConsumer(IMediator mediator, ICurrentUserService currentUserService, IRequestClient<GetUser> requestClient, ILogger<MessengerUserCreatedConsumer> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _requestClient = requestClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        try 
        {
            var message = context.Message;

            _currentUserService.SetCurrentUser(message.CreatedById);

            var messageR = await _requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId, (message.CreatedById)));
            var message2 = messageR.Message;

            var result = await _mediator.Send(new CreateUserCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, message2.Email));
        }
        catch(Exception e) 
        {
        _logger.LogError(e, "FOO"); 
        }
    }
}

public class MessengerUserDeletedConsumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public MessengerUserDeletedConsumer(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.DeletedById);

        await _mediator.Send(new DeleteUserCommand(message.UserId));
    }
}

public class MessengerUserUpdatedConsumer : IConsumer<UserUpdated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetUser> _requestClient;
    private readonly ICurrentUserService _currentUserService;

    public MessengerUserUpdatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _requestClient = requestClient;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.UpdatedById);

        var messageR = await _requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId, message.UpdatedById));
        var message2 = messageR.Message;

        var result = await _mediator.Send(new UpdateUserCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, message2.Email));
    }
}
