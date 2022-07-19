using MassTransit;

using MediatR;

using YourBrand.HumanResources.Contracts;
using YourBrand.IdentityService.Application.Common.Interfaces;
using YourBrand.IdentityService.Application.Users.Commands;
namespace YourBrand.IdentityService.Consumers;

public class IdentityServicePersonCreatedConsumer : IConsumer<PersonCreated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetPerson> _requestClient;
    private readonly ILogger<IdentityServicePersonCreatedConsumer> _logger;
    private readonly ICurrentUserService _currentUserService;

    public IdentityServicePersonCreatedConsumer(IMediator mediator, ICurrentUserService currentUserService, IRequestClient<GetPerson> requestClient, ILogger<IdentityServicePersonCreatedConsumer> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _requestClient = requestClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PersonCreated> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.CreatedById);

        var messageR = await _requestClient.GetResponse<GetPersonResponse>(new GetPerson(message.PersonId, (message.CreatedById)));
        var message2 = messageR.Message;

        var result = await _mediator.Send(new CreateUserCommand(message2.FirstName, message2.LastName, message2.DisplayName, null, message2.SSN, message2.Email, null));
    }
}

public class IdentityServicePersonDeletedConsumer : IConsumer<PersonDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public IdentityServicePersonDeletedConsumer(IMediator mediator, ICurrentUserService currentUserService)
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

public class IdentityServicePersonUpdatedConsumer : IConsumer<PersonUpdated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetPerson> _requestClient;
    private readonly ICurrentUserService _currentUserService;

    public IdentityServicePersonUpdatedConsumer(IMediator mediator, IRequestClient<GetPerson> requestClient, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _requestClient = requestClient;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<PersonUpdated> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.UpdatedById);

        var messageR = await _requestClient.GetResponse<GetPersonResponse>(new GetPerson(message.PersonId, message.UpdatedById));
        var message2 = messageR.Message;

        var result = await _mediator.Send(new UpdateUserDetailsCommand(message2.PersonId, message2.FirstName, message2.LastName, message2.DisplayName, message2.SSN, message2.Email));
    }
}
