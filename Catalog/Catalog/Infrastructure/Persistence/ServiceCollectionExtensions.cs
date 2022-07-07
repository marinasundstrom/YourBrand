using YourBrand.Catalog.Domain;
using YourBrand.Catalog.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using YourBrand.Catalog.Infrastructure.Persistence.Interceptors;

namespace YourBrand.Catalog.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = YourBrand.Catalog.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, "Catalog")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<CatalogContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure());
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<ICatalogContext>(sp => sp.GetRequiredService<CatalogContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        return services;
    }
}