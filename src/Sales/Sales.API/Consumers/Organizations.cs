using MassTransit;

using MediatR;

using YourBrand.IdentityManagement.Contracts;
using YourBrand.Sales.Features.OrderManagement.Organizations;
using YourBrand.Sales.Features.OrderManagement.Users;
using YourBrand.Tenancy;

namespace YourBrand.Sales.Consumers;

public class SalesOrganizationCreatedConsumer(IMediator mediator, ISettableTenantContext tenantContext, ILogger<SalesOrganizationCreatedConsumer> logger) : IConsumer<OrganizationCreated>
{
    public async Task Consume(ConsumeContext<OrganizationCreated> context)
    {
        try
        {
            var message = context.Message;

            SalesSyncTenantContext.SetTenantContext(tenantContext, message.TenantId);

            var result = await mediator.Send(new Sales.Features.OrderManagement.Organizations.CreateOrganization(message.OrganizationId, message.Name, message.TenantId));
        }
        catch (Exception e)
        {
            logger.LogError(e, "FOO");
        }
    }
}

public class SalesOrganizationDeletedConsumer(IMediator mediator, ISettableTenantContext tenantContext) : IConsumer<OrganizationDeleted>
{
    public async Task Consume(ConsumeContext<OrganizationDeleted> context)
    {
        var message = context.Message;

        SalesSyncTenantContext.SetTenantContext(tenantContext, message.TenantId ?? SalesSyncTenantContext.GetTenantIdHeader(context));

        await mediator.Send(new DeleteOrganization(message.OrganizationId));
    }
}


public class SalesOrganizationUpdatedConsumer(IMediator mediator, IRequestClient<GetOrganization> requestClient, ISettableTenantContext tenantContext) : IConsumer<OrganizationUpdated>
{
    public async Task Consume(ConsumeContext<OrganizationUpdated> context)
    {
        var message = context.Message;

        var tenantId = message.TenantId ?? SalesSyncTenantContext.GetTenantIdHeader(context);
        SalesSyncTenantContext.SetTenantContextIfAvailable(tenantContext, tenantId);

        var messageR = await requestClient.GetResponse<GetOrganizationResponse>(new GetOrganization(message.OrganizationId, string.Empty));
        var message2 = messageR.Message;

        SalesSyncTenantContext.SetTenantContext(tenantContext, tenantId ?? message2.TenantId);

        var result = await mediator.Send(new UpdateOrganization(message2.Id, message2.Name));
    }
}

public class SalesOrganizationUserAddedConsumer(IMediator mediator, ISettableTenantContext tenantContext) : IConsumer<OrganizationUserAdded>
{
    public async Task Consume(ConsumeContext<OrganizationUserAdded> context)
    {
        var message = context.Message;

        SalesSyncTenantContext.SetTenantContext(tenantContext, message.TenantId);

        var result = await mediator.Send(new AddUserToOrganization(message.OrganizationId, message.UserId));
    }
}
