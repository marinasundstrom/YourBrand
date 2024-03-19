using MassTransit;

using MediatR;

using YourBrand.IdentityManagement.Contracts;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Users.Commands;
using YourBrand.Tenancy;
using YourBrand.Identity;

namespace YourBrand.TimeReport.Consumers;

public class TimeReportUserCreatedConsumer : IConsumer<UserCreated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetUser> _requestClient;
    private readonly ICurrentUserService _currentUserService;

    public TimeReportUserCreatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ICurrentUserService currentUserService)
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

        var result = await _mediator.Send(new CreateUserCommand(message2.UserId, message2.OrganizationId, message2.FirstName, message2.LastName, message2.DisplayName, "SSN", message2.Email));
    }
}

public class TimeReportUserDeletedConsumer : IConsumer<UserDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public TimeReportUserDeletedConsumer(IMediator mediator, ICurrentUserService currentUserService)
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

public class TimeReportUserUpdatedConsumer : IConsumer<UserUpdated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetUser> _requestClient;
    private readonly ICurrentUserService _currentUserService;

    public TimeReportUserUpdatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _requestClient = requestClient;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.UpdatedById);

        var messageR = await _requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId, (message.UpdatedById)));
        var message2 = messageR.Message;

        var result = await _mediator.Send(new UpdateUserCommand(message2.UserId, message2.FirstName, message2.LastName, message2.DisplayName, "SSN", message2.Email));
    }
}
