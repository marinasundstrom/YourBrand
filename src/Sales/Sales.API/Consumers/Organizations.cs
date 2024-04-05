using MassTransit;

using MediatR;

using YourBrand.IdentityManagement.Contracts;
using YourBrand.Sales.Features.OrderManagement.Organizations;
using YourBrand.Sales.Features.OrderManagement.Users;

namespace YourBrand.Sales.Consumers;

public class SalesOrganizationCreatedConsumer(IMediator mediator, ITenantContext tenantContext, IUserContext userContext, IRequestClient<GetOrganization> requestClient, ILogger<SalesOrganizationCreatedConsumer> logger) : IConsumer<OrganizationCreated>
{
    public async Task Consume(ConsumeContext<OrganizationCreated> context)
    {
        try
        {
            var message = context.Message;

            tenantContext.SetTenantId(message.TenantId);

            //_userContext.SetCurrentUser(message.CreatedById);

            var result = await mediator.Send(new Sales.Features.OrderManagement.Organizations.CreateOrganization(message.OrganizationId, message.Name, message.TenantId));
        }
        catch (Exception e)
        {
            logger.LogError(e, "FOO");
        }
    }
}

public class SalesOrganizationDeletedConsumer(IMediator mediator, ITenantContext tenantContext, IUserContext userContext) : IConsumer<OrganizationDeleted>
{
    public async Task Consume(ConsumeContext<OrganizationDeleted> context)
    {
        var message = context.Message;

        //tenantContext.SetTenantId(message.TenantId);

        //_userContext.SetCurrentUser(message.DeletedById);

        await mediator.Send(new DeleteUser(message.OrganizationId));
    }
}


public class SalesOrganizationUpdatedConsumer(IMediator mediator, IRequestClient<GetOrganization> requestClient, ITenantContext tenantContext, IUserContext userContext) : IConsumer<OrganizationUpdated>
{
    public async Task Consume(ConsumeContext<OrganizationUpdated> context)
    {
        var message = context.Message;

        //tenantContext.SetTenantId(message.TenantId);

        //_userContext.SetCurrentUser(message.UpdatedById);

        var messageR = await requestClient.GetResponse<GetOrganizationResponse>(new GetUser(message.OrganizationId, message.UpdatedById));
        var message2 = messageR.Message;

        var result = await mediator.Send(new UpdateOrganization(message2.Id, message2.Name));
    }
}

public class SalesOrganizationUserAddedConsumer(IMediator mediator, ITenantContext tenantContext) : IConsumer<OrganizationUserAdded>
{
    public async Task Consume(ConsumeContext<OrganizationUserAdded> context)
    {
        var message = context.Message;

        tenantContext.SetTenantId(message.TenantId);

        var result = await mediator.Send(new AddUserToOrganization(message.OrganizationId, message.UserId));
    }
}