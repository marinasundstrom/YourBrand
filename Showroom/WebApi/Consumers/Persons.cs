using MassTransit;

using MediatR;

using YourBrand.HumanResources.Contracts;
using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Application.Users.Commands;
namespace YourBrand.Showroom.Consumers;

public class ShowroomPersonCreatedConsumer : IConsumer<PersonCreated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetPerson> _requestClient;
    private readonly ILogger<ShowroomPersonCreatedConsumer> _logger;
    private readonly ICurrentUserService _currentUserService;

    public ShowroomPersonCreatedConsumer(IMediator mediator, ICurrentUserService currentUserService, IRequestClient<GetPerson> requestClient, ILogger<ShowroomPersonCreatedConsumer> logger)
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

        var result = await _mediator.Send(new CreateUserCommand(message2.PersonId, message2.FirstName, message2.LastName, message2.DisplayName, message2.SSN, message2.Email));
    }
}

public class ShowroomPersonDeletedConsumer : IConsumer<PersonDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public ShowroomPersonDeletedConsumer(IMediator mediator, ICurrentUserService currentUserService)
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

public class ShowroomPersonUpdatedConsumer : IConsumer<PersonUpdated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetPerson> _requestClient;
    private readonly ICurrentUserService _currentUserService;

    public ShowroomPersonUpdatedConsumer(IMediator mediator, IRequestClient<GetPerson> requestClient, ICurrentUserService currentUserService)
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

        var result = await _mediator.Send(new UpdateUserCommand(message2.PersonId, message2.FirstName, message2.LastName, message2.DisplayName, message2.SSN, message2.Email));
    }
}
