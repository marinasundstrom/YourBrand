using MassTransit;

using YourBrand.Tenancy;

namespace YourBrand.Integration;

public class AddTenantIdSendFilter<T>(ITenantContext tenantContext) :
    IFilter<SendContext<T>>
    where T : class
{
    public void Probe(ProbeContext context)
    {
    }

    public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
    {
        context.Headers.Set(Constants.TenantId, tenantContext.TenantId);

        return next.Send(context);
    }
}
