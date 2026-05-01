using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Infrastructure.Persistence.Interceptors;
using YourBrand.Tenancy;

namespace YourBrand.Invoicing.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string DatabaseName = "Invoicing";

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var databasePerTenant = configuration.IsDatabasePerTenantEnabled("Invoicing");

        services.AddDbContext<InvoicingContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString!, o => o.EnableRetryOnFailure());

            options.AddInterceptors(
                sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

            options.UseTenantDatabasePerTenant<InvoicingContext>(sp, databasePerTenant);

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IInvoicingContext>(sp => sp.GetRequiredService<InvoicingContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddTenantDatabasePerTenant(databasePerTenant, builder =>
        {
            builder.ForContext<InvoicingContext>()
                .WithDatabasePrefix(DatabaseName);
        });

        return services;
    }
}
