using YourBrand.Invoices.Domain;
using YourBrand.Invoices.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Invoices.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = YourBrand.Invoices.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, "Invoices")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<InvoicesContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure());
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IInvoicesContext>(sp => sp.GetRequiredService<InvoicesContext>());

        return services;
    }
}