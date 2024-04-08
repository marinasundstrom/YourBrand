using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Tenancy;

public static class ServicesExtensions
{
    public static IServiceCollection AddTenantContext(this IServiceCollection services)
    {
        services.AddScoped<ISettableTenantContext, TenantContext>();
        services.AddScoped<ITenantContext>(sp => sp.GetRequiredService<ISettableTenantContext>());

        return services;
    }
}