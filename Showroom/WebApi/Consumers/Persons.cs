using MassTransit;

using MediatR;

using YourBrand.UserManagement.Contracts;
using YourBrand.Identity;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Users.Commands;
namespace YourBrand.Showroom.Consumers;

public class ShowroomUserCreatedConsumer : IConsumer<UserCreated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetUser> _requestClient;
    private readonly ILogger<ShowroomUserCreatedConsumer> _logger;
    private readonly ICurrentUserService _currentUserService;

    public ShowroomUserCreatedConsumer(IMediator mediator, ICurrentUserService currentUserService, IRequestClient<GetUser> requestClient, ILogger<ShowroomUserCreatedConsumer> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _requestClient = requestClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.CreatedById);

        var messageR = await _requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId, (message.CreatedById)));
        var message2 = messageR.Message;

        var result = await _mediator.Send(new CreateUserCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, "SSN", message2.Email));
    }
}

public class ShowroomUserDeletedConsumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public ShowroomUserDeletedConsumer(IMediator mediator, ICurrentUserService currentUserService)
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

public class ShowroomUserUpdatedConsumer : IConsumer<UserUpdated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetUser> _requestClient;
    private readonly ICurrentUserService _currentUserService;

    public ShowroomUserUpdatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ICurrentUserService currentUserService)
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

        var result = await _mediator.Send(new UpdateUserCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, "SSN", message2.Email));
    }
}
