using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;
using YourBrand.Marketing.Infrastructure.Persistence.Interceptors;
using YourBrand.Tenancy;

namespace YourBrand.Marketing.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string DatabaseName = "Marketing";

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var databasePerTenant = configuration.IsDatabasePerTenantEnabled("Marketing");

        services.AddDbContext<MarketingContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString!, o => o.EnableRetryOnFailure());

            options.AddInterceptors(
                sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

            options.UseTenantDatabasePerTenant<MarketingContext>(sp, databasePerTenant);

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IMarketingContext>(sp => sp.GetRequiredService<MarketingContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddTenantDatabasePerTenant(databasePerTenant, builder =>
        {
            builder.ForContext<MarketingContext>()
                .WithDatabasePrefix(DatabaseName);
        });

        return services;
    }
}
