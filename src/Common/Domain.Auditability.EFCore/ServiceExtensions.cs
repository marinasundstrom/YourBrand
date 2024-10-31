using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Auditability;

public static class ServiceExtensions
{
    public static IServiceCollection AddAuditabilityInterceptor(this IServiceCollection services)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        return services;
    }
}