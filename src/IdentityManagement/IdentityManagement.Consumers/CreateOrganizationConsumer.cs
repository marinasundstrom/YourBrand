using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Application.Organizations.Commands;
using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.IdentityManagement.Consumers;

public class CreateOrganizationConsumer : IConsumer<CreateOrganization>
{
    private readonly IMediator _mediator;
    private readonly IUserContext _userContext;

    public CreateOrganizationConsumer(IMediator mediator, IUserContext userContext)
    {
        _mediator = mediator;
        _userContext = userContext;
    }

    public async Task Consume(ConsumeContext<CreateOrganization> context)
    {
        var message = context.Message;

        var organization = await _mediator.Send(new CreateOrganizationCommand(message.Name, message.FriendlyName));

        await context.RespondAsync(new CreateOrganizationResponse(organization.Id, organization.Name, organization.FriendlyName));
    }
}