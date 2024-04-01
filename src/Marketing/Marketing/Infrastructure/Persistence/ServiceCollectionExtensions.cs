using Microsoft.EntityFrameworkCore;

using YourBrand.Marketing.Domain;
using YourBrand.Marketing.Infrastructure.Persistence.Interceptors;

namespace YourBrand.Marketing.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = YourBrand.Marketing.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, "Marketing")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<MarketingContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure());
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IMarketingContext>(sp => sp.GetRequiredService<MarketingContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        return services;
    }
}