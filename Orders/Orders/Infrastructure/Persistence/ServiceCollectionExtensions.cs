using YourBrand.Orders.Domain;
using YourBrand.Orders.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using YourBrand.Orders.Infrastructure.Persistence.Interceptors;

namespace YourBrand.Orders.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = YourBrand.Orders.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, "Orders")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<OrdersContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure());
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IOrdersContext>(sp => sp.GetRequiredService<OrdersContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        return services;
    }
}