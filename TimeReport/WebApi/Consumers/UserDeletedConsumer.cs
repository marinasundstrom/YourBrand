
using MassTransit;

using MediatR;

using YourBrand.IdentityService.Client;
using YourBrand.IdentityService.Contracts;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Users.Commands;

namespace YourBrand.TimeReport.Consumers;

public class TimeReportUserDeletedConsumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;
    private readonly IUsersClient _usersClient;
    private readonly ICurrentUserService _currentUserService;

    public TimeReportUserDeletedConsumer(IMediator mediator, IUsersClient usersClient, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _usersClient = usersClient;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.DeletedById);

        var result = await _mediator.Send(new DeleteUserCommand(message.UserId));
    }
}