using MassTransit;

using MediatR;

using YourBrand.IdentityService.Contracts;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Users.Commands;
namespace YourBrand.Showroom.Consumers;

public class UserCreated1Consumer : IConsumer<UserCreated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetUser> _requestClient;
    private readonly ILogger<UserCreated1Consumer> _logger;
    private readonly ICurrentUserService _currentUserService;

    public UserCreated1Consumer(IMediator mediator, ICurrentUserService currentUserService, IRequestClient<GetUser> requestClient, ILogger<UserCreated1Consumer> logger)
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

        var result = await _mediator.Send(new CreateUserCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, message2.SSN, message2.Email));
    }
}