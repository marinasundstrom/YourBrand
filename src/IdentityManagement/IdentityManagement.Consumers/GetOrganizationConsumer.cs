using MassTransit;

using MediatR;

using YourBrand.IdentityManagement.Contracts;
using YourBrand.Identity;

namespace YourBrand.IdentityManagement.Consumers;

public class GetOrganizationConsumer : IConsumer<GetOrganization>
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUserService;

    public GetOrganizationConsumer(IMediator mediator, ICurrentUserService currentUserService)
    {
        _mediator = mediator;
        _currentUserService = currentUserService;
    }

    public async Task Consume(ConsumeContext<GetOrganization> context)
    {
        var message = context.Message;

        var organization = await _mediator.Send(new YourBrand.IdentityManagement.Application.Organizations.Queries.GetOrganizationQuery(message.OrganizationId));

        await context.RespondAsync(new GetOrganizationResponse(organization.Id, organization.Tenant.Id, organization.Name, organization.FriendlyName));
    }
}
