using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Organizations.Commands;
using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.IdentityManagement.Consumers;

public class CreateOrganizationConsumer(IMediator mediator) : IConsumer<CreateOrganization>
{
    public async Task Consume(ConsumeContext<CreateOrganization> context)
    {
        var message = context.Message;

        var organization = await mediator.Send(new CreateOrganizationCommand(message.Name, message.FriendlyName));

        await context.RespondAsync(new CreateOrganizationResponse(organization.Id, organization.Name, organization.FriendlyName));
    }
}
