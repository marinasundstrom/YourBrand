using YourBrand.Customers.Domain;
using YourBrand.Customers.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Customers.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = YourBrand.Customers.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, "Customers")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<CustomersContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure());
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<ICustomersContext>(sp => sp.GetRequiredService<CustomersContext>());

        return services;
    }
}