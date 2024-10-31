using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Auditability;

public static class DbContextOptionsBuilderExtensions
{
    public static DbContextOptionsBuilder UseAuditabilityInterceptor(this DbContextOptionsBuilder options, IServiceProvider serviceProvider)
    {
        options.AddInterceptors(
            serviceProvider.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

        return options;
    }
}

public static class ServiceExtensions
{
    public static IServiceCollection AddAuditabilityInterceptor(this IServiceCollection services)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        return services;
    }
}