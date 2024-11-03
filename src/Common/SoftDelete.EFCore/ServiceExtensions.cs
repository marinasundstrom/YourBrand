using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace YourBrand.Auditability;

public static class ServiceExtensions
{
    public static IServiceCollection AddSoftDeleteInterceptor(this IServiceCollection services)
    {
        services.TryAddScoped<SoftDeletableEntitySaveChangesInterceptor>();
        return services;
    }
}
