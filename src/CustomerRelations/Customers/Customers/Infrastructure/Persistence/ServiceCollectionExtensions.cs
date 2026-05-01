using Microsoft.EntityFrameworkCore;

using YourBrand.Customers.Domain;
using YourBrand.Customers.Infrastructure.Persistence.Interceptors;
using YourBrand.Tenancy;

namespace YourBrand.Customers.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string DatabaseName = "Customers";

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var databasePerTenant = configuration.IsDatabasePerTenantEnabled("Customers");

        services.AddDbContext<CustomersContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString!, o => o.EnableRetryOnFailure());

            options.AddInterceptors(
                sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

            options.UseTenantDatabasePerTenant<CustomersContext>(sp, databasePerTenant);

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<ICustomersContext>(sp => sp.GetRequiredService<CustomersContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddTenantDatabasePerTenant(databasePerTenant, builder =>
        {
            builder.ForContext<CustomersContext>()
                .WithDatabasePrefix(DatabaseName);
        });

        return services;
    }
}
