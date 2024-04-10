using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;
using YourBrand.Marketing.Infrastructure.Persistence.Interceptors;

namespace YourBrand.Marketing.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<MarketingContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString!, o => o.EnableRetryOnFailure());

            options.AddInterceptors(
                sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IMarketingContext>(sp => sp.GetRequiredService<MarketingContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        return services;
    }
}