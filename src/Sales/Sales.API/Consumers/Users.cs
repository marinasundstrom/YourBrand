using MassTransit;

using MediatR;

using YourBrand.Integration;
using YourBrand.IdentityManagement.Contracts;
using YourBrand.Sales.Features.OrderManagement.Users;
using YourBrand.Tenancy;

namespace YourBrand.Sales.Consumers;

public class SalesUserCreatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ISettableTenantContext tenantContext, ILogger<SalesUserCreatedConsumer> logger) : IConsumer<UserCreated>
{
    public async Task Consume(ConsumeContext<UserCreated> context)
    {
        try
        {
            var message = context.Message;

            SalesSyncTenantContext.SetTenantContext(tenantContext, message.TenantId);

            var messageR = await requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId));
            var message2 = messageR.Message;

            var result = await mediator.Send(new Sales.Features.OrderManagement.Users.CreateUser($"{message2.FirstName} {message2.LastName}", message2.Email, message.TenantId, message2.UserId));
        }
        catch (Exception e)
        {
            logger.LogError(e, "FOO");
        }
    }
}

public class SalesUserDeletedConsumer(IMediator mediator, ISettableTenantContext tenantContext) : IConsumer<UserDeleted>
{
    public async Task Consume(ConsumeContext<UserDeleted> context)
    {
        var message = context.Message;

        SalesSyncTenantContext.SetTenantContext(tenantContext, message.TenantId ?? SalesSyncTenantContext.GetTenantIdHeader(context));

        await mediator.Send(new DeleteUser(message.UserId));
    }
}


public class SalesUserUpdatedConsumer(IMediator mediator, IRequestClient<GetUser> requestClient, ISettableTenantContext tenantContext) : IConsumer<UserUpdated>
{
    public async Task Consume(ConsumeContext<UserUpdated> context)
    {
        var message = context.Message;

        var tenantId = message.TenantId ?? SalesSyncTenantContext.GetTenantIdHeader(context);
        SalesSyncTenantContext.SetTenantContextIfAvailable(tenantContext, tenantId);

        var messageR = await requestClient.GetResponse<GetUserResponse>(new GetUser(message.UserId));
        var message2 = messageR.Message;

        SalesSyncTenantContext.SetTenantContext(tenantContext, tenantId ?? message2.TenantId);

        var result = await mediator.Send(new UpdateUser(message2.UserId, $"{message2.FirstName} {message2.LastName}", message2.Email));
    }
}

static class SalesSyncTenantContext
{
    public static string? GetTenantIdHeader<T>(ConsumeContext<T> context)
        where T : class
        => context.Headers.Get<string>(Constants.TenantId);

    public static void SetTenantContext(ISettableTenantContext tenantContext, string? tenantId)
    {
        if (string.IsNullOrWhiteSpace(tenantId))
        {
            throw new InvalidOperationException("Tenant id is required to sync Sales data.");
        }

        tenantContext.SetTenantId(tenantId);
    }

    public static void SetTenantContextIfAvailable(ISettableTenantContext tenantContext, string? tenantId)
    {
        if (!string.IsNullOrWhiteSpace(tenantId))
        {
            tenantContext.SetTenantId(tenantId);
        }
    }
}
