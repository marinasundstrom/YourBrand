using YourBrand.IdentityManagement.Contracts;
using YourBrand.Sales.Features.Services;
using YourBrand.Sales.Features.OrderManagement.Users;

using MassTransit;

using MediatR;

namespace YourBrand.Sales.Consumers;

public class SalesUserCreatedConsumer : IConsumer<UserCreated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetUser> _requestClient;
    private readonly ILogger<SalesUserCreatedConsumer> _logger;
    private readonly ICurrentUserService _currentUserService;

    public SalesUserCreatedConsumer(IMediator mediator, ICurrentUserService currentUserService, IRequestClient<GetUser> requestClient, ILogger<SalesUserCreatedConsumer> logger)
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

            //_currentUserService.SetCurrentUser(message.CreatedById);

            var messageR = await _requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId, (message.CreatedById)));
            var message2 = messageR.Message;

            var result = await _mediator.Send(new Sales.Features.OrderManagement.Users.CreateUser($"{message2.FirstName} {message2.LastName}", message2.Email, message.OrganizationId, message2.UserId));
        }
        catch(Exception e) 
        {
            _logger.LogError(e, "FOO"); 
        }
    }
}

public class SalesUserDeletedConsumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public SalesUserDeletedConsumer(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        //_currentUserService.SetCurrentUser(message.DeletedById);

        await _mediator.Send(new DeleteUser(message.UserId));
    }
}


public class SalesUserUpdatedConsumer : IConsumer<UserUpdated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetUser> _requestClient;
    private readonly ICurrentUserService _currentUserService;

    public SalesUserUpdatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _requestClient = requestClient;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var message = context.Message;

        //_currentUserService.SetCurrentUser(message.UpdatedById);

        var messageR = await _requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId, message.UpdatedById));
        var message2 = messageR.Message;

        var result = await _mediator.Send(new UpdateUser(message2.UserId, $"{message2.FirstName} {message2.LastName}", message2.Email));
    }
}