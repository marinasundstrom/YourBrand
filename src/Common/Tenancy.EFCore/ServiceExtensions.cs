using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Tenancy;

public static class ServiceExtensions
{
    public static IServiceCollection AddTenancyInterceptor(this IServiceCollection services)
    {
        services.AddScoped<SetTenantSaveChangesInterceptor>();
        return services;
    }
}