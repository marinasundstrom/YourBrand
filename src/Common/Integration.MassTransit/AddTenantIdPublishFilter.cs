using MassTransit;

using YourBrand.Tenancy;

namespace YourBrand.Integration;

public sealed class AddTenantIdPublishFilter<T>(ITenantContext tenantContext) :
    IFilter<PublishContext<T>>
    where T : class
{
    public void Probe(ProbeContext context)
    {
    }

    public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
    {
        context.Headers.Set(Constants.TenantId, tenantContext.TenantId);

        return next.Send(context);
    }
}