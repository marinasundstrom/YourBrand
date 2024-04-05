using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Tenancy;

public static class ServicesExtensions
{
    public static IServiceCollection AddTenantContext(this IServiceCollection services)
    {
        services.AddScoped<ITenantContext, TenantContext>();

        return services;
    }
}