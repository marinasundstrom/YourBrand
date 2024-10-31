using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace YourBrand.Auditability;

public static class ServiceExtensions
{
    public static IServiceCollection AddAuditabilityInterceptor(this IServiceCollection services)
    {
        services.TryAddScoped<AuditableEntitySaveChangesInterceptor>();
        return services;
    }
}