using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Infrastructure.Persistence.Interceptors;

namespace YourBrand.Invoicing.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = YourBrand.Invoicing.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, "Invoicing")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<InvoicingContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure());
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IInvoicingContext>(sp => sp.GetRequiredService<InvoicingContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        return services;
    }
}