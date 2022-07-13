using YourBrand.Warehouse.Domain;
using YourBrand.Warehouse.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using YourBrand.Warehouse.Infrastructure.Persistence.Interceptors;

namespace YourBrand.Warehouse.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = YourBrand.Warehouse.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, "Warehouse")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<WarehouseContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure());
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IWarehouseContext>(sp => sp.GetRequiredService<WarehouseContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        return services;
    }
}