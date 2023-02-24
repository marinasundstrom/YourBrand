using YourBrand.HumanResources.Contracts;
using YourBrand.ApiKeys.Application.Common.Interfaces;
using YourBrand.ApiKeys.Application.Users.Commands;

using MassTransit;

using MediatR;
using YourBrand.Identity;

namespace YourBrand.ApiKeys.Consumers;

public class ApiKeysPersonCreatedConsumer : IConsumer<PersonCreated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetPerson> _requestClient;
    private readonly ILogger<ApiKeysPersonCreatedConsumer> _logger;
    private readonly ICurrentUserService _currentUserService;

    public ApiKeysPersonCreatedConsumer(IMediator mediator, ICurrentUserService currentUserService, IRequestClient<GetPerson> requestClient, ILogger<ApiKeysPersonCreatedConsumer> logger)
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

            var result = await _mediator.Send(new CreateUserCommand(message2.PersonId, message2.FirstName, message2.LastName, message2.DisplayName, message2.Email));
        }
        catch(Exception e) 
        {
        _logger.LogError(e, "FOO"); 
        }
    }
}

public class ApiKeysPersonDeletedConsumer : IConsumer<PersonDeleted>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public ApiKeysPersonDeletedConsumer(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<PersonDeleted> context)
    {
        var message = context.Message;

        _currentUserService.SetCurrentUser(message.DeletedById);

        await _mediator.Send(new DeleteUserCommand(message.PersonId));
    }
}

public class ApiKeysPersonUpdatedConsumer : IConsumer<PersonUpdated>
{
    private readonly IMediator _mediator;
    private readonly IRequestClient<GetPerson> _requestClient;
    private readonly ICurrentUserService _currentUserService;

    public ApiKeysPersonUpdatedConsumer(IMediator mediator, IRequestClient<GetPerson> requestClient, ICurrentUserService currentUserService)
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

        var result = await _mediator.Send(new UpdateUserCommand(message2.PersonId, message2.FirstName, message2.LastName, message2.DisplayName, message2.Email));
    }
}
