using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Infrastructure.Persistence.Interceptors;

namespace YourBrand.Inventory.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<InventoryContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString!, o => o.EnableRetryOnFailure());

            options.AddInterceptors(
                sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });
        
        services.AddScoped<IInventoryContext>(sp => sp.GetRequiredService<InventoryContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        return services;
    }
}