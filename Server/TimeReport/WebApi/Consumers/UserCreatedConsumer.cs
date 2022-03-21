using System;

using MassTransit;

using MediatR;

using YourCompany.IdentityService.Client;
using YourCompany.IdentityService.Contracts;
using YourCompany.TimeReport.Application;
using YourCompany.TimeReport.Application.Common.Interfaces;
using YourCompany.TimeReport.Application.Users.Commands;

namespace YourCompany.TimeReport.Consumers;

public class UserCreatedConsumer : IConsumer<UserCreated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetUser> _requestClient;
    private readonly ICurrentUserService _currentUserService;

    public UserCreatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _requestClient = requestClient;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.CreatedById);

        var messageR = await _requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId, message.CreatedById));
        var message2 = messageR.Message;

        var result = await _mediator.Send(new CreateUserCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, message2.SSN, message2.Email));
    }
}
