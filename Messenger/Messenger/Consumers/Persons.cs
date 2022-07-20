using YourBrand.HumanResources.Contracts;
using YourBrand.Messenger.Application.Common.Interfaces;
using YourBrand.Messenger.Application.Users.Commands;

using MassTransit;

using MediatR;
using YourBrand.Identity;

namespace YourBrand.Messenger.Consumers;

public class MessengerPersonCreatedConsumer : IConsumer<PersonCreated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetPerson> _requestClient;
    private readonly ILogger<MessengerPersonCreatedConsumer> _logger;
    private readonly ICurrentUserService _currentUserService;

    public MessengerPersonCreatedConsumer(IMediator mediator, ICurrentUserService currentPersonService, IRequestClient<GetPerson> requestClient, ILogger<MessengerPersonCreatedConsumer> logger)
    {
        _mediator = mediator;
        _currentUserService = currentPersonService;
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

            var result = await _mediator.Send(new CreateUserCommand(message2.PersonId, message2.FirstName, message2.LastName, message2.DisplayName, message2.Email));
        }
        catch(Exception e) 
        {
        _logger.LogError(e, "FOO"); 
        }
    }
}

public class MessengerPersonDeletedConsumer : IConsumer<PersonDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public MessengerPersonDeletedConsumer(IMediator mediator, ICurrentUserService currentPersonService)
    {
        _mediator = mediator;
        _currentUserService = currentPersonService;
    }

    public async Task Consume(ConsumeContext<PersonDeleted> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.DeletedById);

        var result = await _mediator.Send(new DeleteUserCommand(message.PersonId));
    }
}

public class MessengerPersonUpdatedConsumer : IConsumer<PersonUpdated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetPerson> _requestClient;
    private readonly ICurrentUserService _currentUserService;

    public MessengerPersonUpdatedConsumer(IMediator mediator, IRequestClient<GetPerson> requestClient, ICurrentUserService currentPersonService)
    {
        _mediator = mediator;
        _requestClient = requestClient;
        _currentUserService = currentPersonService;
    }

    public async Task Consume(ConsumeContext<PersonUpdated> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.UpdatedById);

        var messageR = await _requestClient.GetResponse<GetPersonResponse>(new GetPerson(message.PersonId, message.UpdatedById));
        var message2 = messageR.Message;

        var result = await _mediator.Send(new UpdateUserCommand(message2.PersonId, message2.FirstName, message2.LastName, message2.DisplayName, message2.Email));
    }
}
