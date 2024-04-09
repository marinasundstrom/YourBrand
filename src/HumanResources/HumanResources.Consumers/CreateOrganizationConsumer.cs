using MassTransit;

using MediatR;

using YourBrand.HumanResources.Application.Organizations.Commands;
using YourBrand.HumanResources.Contracts;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Consumers;

public class CreateOrganizationConsumer(IMediator mediator, IUserContext currentPersonService) : IConsumer<CreateOrganization>
{
    public async Task Consume(ConsumeContext<CreateOrganization> context)
    {
        var message = context.Message;

        var organization = await mediator.Send(new CreateOrganizationCommand(message.Name, message.FriendlyName));

        await context.RespondAsync(new CreateOrganizationResponse(organization.Id, organization.Name, organization.FriendlyName));
    }
}