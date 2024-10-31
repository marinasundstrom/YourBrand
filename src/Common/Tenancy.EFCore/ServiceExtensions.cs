using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace YourBrand.Tenancy;

public static class ServiceExtensions
{
    public static IServiceCollection AddTenancyInterceptor(this IServiceCollection services)
    {
        services.TryAddScoped<SetTenantSaveChangesInterceptor>();
        return services;
    }
}