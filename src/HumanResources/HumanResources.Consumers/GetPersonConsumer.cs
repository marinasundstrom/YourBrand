using MassTransit;

using MediatR;

using YourBrand.HumanResources.Contracts;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Consumers;

public class GetPersonConsumer(IMediator mediator, IUserContext currentPersonService) : IConsumer<GetPerson>
{
    public async Task Consume(ConsumeContext<GetPerson> context)
    {
        var message = context.Message;

        var person = await mediator.Send(new YourBrand.HumanResources.Application.Persons.Queries.GetPersonQuery(message.PersonId));

        await context.RespondAsync(new GetPersonResponse(person.Id, person.Organization.Id, person.FirstName, person.LastName, person.DisplayName, person.SSN, person.Email));
    }
}