
using MassTransit;

using MediatR;
using YourBrand.HumanResources.Contracts;
using YourBrand.HumanResources.Application.Common.Interfaces;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Consumers;

public class GetPersonConsumer : IConsumer<GetPerson>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentPersonService;

    public GetPersonConsumer(IMediator mediator, ICurrentUserService currentPersonService)
    {
        _mediator = mediator;
        _currentPersonService = currentPersonService;
    }

    public async Task Consume(ConsumeContext<GetPerson> context)
    {
        var message = context.Message;

        var person = await _mediator.Send(new YourBrand.HumanResources.Application.Persons.Queries.GetPersonQuery(message.PersonId));

        await context.RespondAsync(new GetPersonResponse(person.Id, person.Organization.Id, person.FirstName, person.LastName, person.DisplayName, person.SSN, person.Email));
    }
}