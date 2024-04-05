using MassTransit;

using MediatR;

using YourBrand.IdentityManagement.Contracts;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Application.Organizations.Commands;

namespace YourBrand.TimeReport.Consumers;

public class TimeReportOrganizationCreatedConsumer(IMediator mediator, ITenantContext tenantContext, IUserContext currentOrganizationService) : IConsumer<OrganizationCreated>
{
    public async Task Consume(ConsumeContext<OrganizationCreated> context)
    {
        var message = context.Message;

        tenantContext.SetTenantId(message.TenantId);
        currentOrganizationService.SetCurrentUser(message.CreatedById);

        var result = await mediator.Send(new CreateOrganizationCommand(message.OrganizationId, message.Name, null));
    }
}

public class TimeReportOrganizationDeletedConsumer(IMediator mediator, ITenantContext tenantContext, IUserContext currentOrganizationService) : IConsumer<OrganizationDeleted>
{
    public async Task Consume(ConsumeContext<OrganizationDeleted> context)
    {
        var message = context.Message;

        //tenantContext.SetTenantId(message.TenantId);
        currentOrganizationService.SetCurrentUser(message.DeletedById);

        await mediator.Send(new DeleteOrganizationCommand(message.OrganizationId));
    }
}

public class TimeReportOrganizationUpdatedConsumer(IMediator mediator, ITenantContext tenantContext, IUserContext currentOrganizationService) : IConsumer<OrganizationUpdated>
{
    public async Task Consume(ConsumeContext<OrganizationUpdated> context)
    {
        var message = context.Message;

        //tenantContext.SetTenantId(message.TenantId);
        currentOrganizationService.SetCurrentUser(message.UpdatedById);

        var result = await mediator.Send(new UpdateOrganizationCommand(message.OrganizationId, message.Name));
    }
}

public class TimeReportOrganizationUserAddedConsumer(IMediator mediator, ITenantContext tenantContext) : IConsumer<OrganizationUserAdded>
{
    public async Task Consume(ConsumeContext<OrganizationUserAdded> context)
    {
        var message = context.Message;

        tenantContext.SetTenantId(message.TenantId);

        var result = await mediator.Send(new AddUserToOrganization(message.OrganizationId, message.UserId));
    }
}