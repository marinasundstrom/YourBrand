using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using YourBrand.Products.Infrastructure.Persistence.Interceptors;

namespace YourBrand.Products.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = YourBrand.Products.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, "Products")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ProductsContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure());
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IProductsContext>(sp => sp.GetRequiredService<ProductsContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        return services;
    }
}