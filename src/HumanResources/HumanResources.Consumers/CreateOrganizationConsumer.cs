using MassTransit;

using MediatR;

using YourBrand.HumanResources.Application.Organizations.Commands;
using YourBrand.HumanResources.Contracts;
using YourBrand.Identity;

namespace YourBrand.HumanResources.Consumers;

public class CreateOrganizationConsumer : IConsumer<CreateOrganization>
{
    private readonly IMediator _mediator;
    private readonly IUserContext _currentPersonService;

    public CreateOrganizationConsumer(IMediator mediator, IUserContext currentPersonService)
    {
        _mediator = mediator;
        _currentPersonService = currentPersonService;
    }

    public async Task Consume(ConsumeContext<CreateOrganization> context)
    {
        var message = context.Message;

        var organization = await _mediator.Send(new CreateOrganizationCommand(message.Name, message.FriendlyName));

        await context.RespondAsync(new CreateOrganizationResponse(organization.Id, organization.Name, organization.FriendlyName));
    }
}