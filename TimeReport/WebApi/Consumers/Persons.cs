using MassTransit;

using MediatR;

using YourBrand.IdentityService.Client;
using YourBrand.HumanResources.Contracts;
using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Application.Users.Commands;
using YourBrand.Tenancy;
using YourBrand.Identity;

namespace YourBrand.TimeReport.Consumers;

public class TimeReportPersonCreatedConsumer : IConsumer<PersonCreated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetPerson> _requestClient;
    private readonly ICurrentUserService _currentUserService;

    public TimeReportPersonCreatedConsumer(IMediator mediator, IRequestClient<GetPerson> requestClient, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _requestClient = requestClient;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<PersonCreated> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.CreatedById);

        var messageR = await _requestClient.GetResponse<GetPersonResponse>(new GetPerson(message.PersonId, message.CreatedById));
        var message2 = messageR.Message;

        var result = await _mediator.Send(new CreateUserCommand(message2.PersonId, message2.OrganizationId, message2.FirstName, message2.LastName, message2.DisplayName, message2.SSN, message2.Email));
    }
}

public class TimeReportPersonDeletedConsumer : IConsumer<PersonDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public TimeReportPersonDeletedConsumer(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<PersonDeleted> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.DeletedById);

        var result = await _mediator.Send(new DeleteUserCommand(message.PersonId));
    }
}

public class TimeReportPersonUpdatedConsumer : IConsumer<PersonUpdated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetPerson> _requestClient;
    private readonly ICurrentUserService _currentUserService;

    public TimeReportPersonUpdatedConsumer(IMediator mediator, IRequestClient<GetPerson> requestClient, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _requestClient = requestClient;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<PersonUpdated> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.UpdatedById);

        var messageR = await _requestClient.GetResponse<GetPersonResponse>(new GetPerson(message.PersonId, (message.UpdatedById)));
        var message2 = messageR.Message;

        var result = await _mediator.Send(new UpdateUserCommand(message2.PersonId, message2.FirstName, message2.LastName, message2.DisplayName, message2.SSN, message2.Email));
    }
}
