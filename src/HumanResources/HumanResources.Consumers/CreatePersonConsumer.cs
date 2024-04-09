using MassTransit;

using MediatR;

using YourBrand.HumanResources.Application.Persons.Commands;
using YourBrand.HumanResources.Contracts;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Consumers;

public class CreatePersonConsumer(IMediator mediator, IUserContext currentPersonService) : IConsumer<CreatePerson>
{
    public async Task Consume(ConsumeContext<CreatePerson> context)
    {
        var message = context.Message;

        var person = await mediator.Send(new CreatePersonCommand(message.OrganizationId, message.FirstName, message.LastName, message.DisplayName, message.Title, message.Role, message.SSN, message.Email, message.DepartmentId, null, message.Password));

        await context.RespondAsync(new CreatePersonResponse(person.Id, person.Organization.Id, person.FirstName, person.LastName, person.DisplayName, person.SSN, person.Email));
    }
}