using MassTransit;

using MediatR;

using YourBrand.UserManagement.Contracts;
using YourBrand.Identity;
using YourBrand.IdentityService.Application.Common.Interfaces;
using YourBrand.IdentityService.Application.Users.Commands;
namespace YourBrand.IdentityService.Consumers;

public class IdentityServiceUserCreatedConsumer : IConsumer<UserCreated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetUser> _requestClient;
    private readonly ILogger<IdentityServiceUserCreatedConsumer> _logger;
    private readonly ICurrentUserService _currentUserService;

    public IdentityServiceUserCreatedConsumer(IMediator mediator, ICurrentUserService currentUserService, IRequestClient<GetUser> requestClient, ILogger<IdentityServiceUserCreatedConsumer> logger)
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

        var result = await _mediator.Send(new CreateUserCommand(message2.UserId, message2.OrganizationId, message2.FirstName, message2.LastName, message2.DisplayName, "Administrator", "SSN", message2.Email, "Abc123!?"));
    }
}

public class IdentityServiceUserDeletedConsumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public IdentityServiceUserDeletedConsumer(IMediator mediator, ICurrentUserService currentUserService)
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

public class IdentityServiceUserUpdatedConsumer : IConsumer<UserUpdated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetUser> _requestClient;
    private readonly ICurrentUserService _currentUserService;

    public IdentityServiceUserUpdatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ICurrentUserService currentUserService)
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

        var result = await _mediator.Send(new UpdateUserDetailsCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, "SSN", message2.Email));
    }
}
