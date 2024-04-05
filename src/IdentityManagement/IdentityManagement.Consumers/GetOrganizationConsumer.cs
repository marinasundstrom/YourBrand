using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.IdentityManagement.Contracts;

namespace YourBrand.IdentityManagement.Consumers;

public class GetOrganizationConsumer : IConsumer<GetOrganization>
{
    private readonly IMediator _mediator;
    private readonly IUserContext _userContext;

    public GetOrganizationConsumer(IMediator mediator, IUserContext userContext)
    {
        _mediator = mediator;
        _userContext = userContext;
    }

    public async Task Consume(ConsumeContext<GetOrganization> context)
    {
        var message = context.Message;

        var organization = await _mediator.Send(new YourBrand.IdentityManagement.Application.Organizations.Queries.GetOrganizationQuery(message.OrganizationId));

        await context.RespondAsync(new GetOrganizationResponse(organization.Id, organization.Tenant.Id, organization.Name, organization.FriendlyName));
    }
}