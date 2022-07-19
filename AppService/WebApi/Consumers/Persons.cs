using MassTransit;

using MediatR;
using YourBrand.HumanResources.Contracts;
using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Users.Commands;
namespace YourBrand.Consumers;

public class AppServicePersonCreatedConsumer : IConsumer<PersonCreated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetPerson> _requestClient;
    private readonly ILogger<AppServicePersonCreatedConsumer> _logger;
    private readonly ICurrentUserService _currentUserService;

    public AppServicePersonCreatedConsumer(IMediator mediator, ICurrentUserService currentUserService, IRequestClient<GetPerson> requestClient, ILogger<AppServicePersonCreatedConsumer> logger)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
        _requestClient = requestClient;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PersonCreated> context)
    {
        try 
        {
            var message = context.Message;

            _currentUserService.SetCurrentUser(message.CreatedById);

            var messageR = await _requestClient.GetResponse<GetPersonResponse>(new GetPerson(message.PersonId, (message.CreatedById)));
            var message2 = messageR.Message;

            var result = await _mediator.Send(new CreateUserCommand(message2.PersonId, message2.FirstName, message2.LastName, message2.DisplayName, message2.SSN, message2.Email));
        }
        catch(Exception e) 
        {
        _logger.LogError(e, "FOO"); 
        }
    }
}

public class AppServicePersonDeletedConsumer : IConsumer<PersonDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public AppServicePersonDeletedConsumer(IMediator mediator, ICurrentUserService currentUserService)
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

public class AppServicePersonUpdatedConsumer : IConsumer<PersonUpdated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetPerson> _requestClient;
    private readonly ICurrentUserService _currentUserService;

    public AppServicePersonUpdatedConsumer(IMediator mediator, IRequestClient<GetPerson> requestClient, ICurrentUserService currentUserService)
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
