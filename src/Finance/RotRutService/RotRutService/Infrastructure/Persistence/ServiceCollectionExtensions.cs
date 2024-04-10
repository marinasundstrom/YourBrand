using Microsoft.EntityFrameworkCore;

using YourBrand.RotRutService.Domain;
using YourBrand.RotRutService.Infrastructure.Persistence.Interceptors;

namespace YourBrand.RotRutService.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<RotRutContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString!, o => o.EnableRetryOnFailure());

            options.AddInterceptors(
                sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IRotRutContext>(sp => sp.GetRequiredService<RotRutContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        return services;
    }
}