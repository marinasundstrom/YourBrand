using YourBrand.RotRutService.Domain;
using YourBrand.RotRutService.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.RotRutService.Infrastructure.Persistence;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = YourBrand.RotRutService.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, "RotRutService")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<RotRutContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString, o => o.EnableRetryOnFailure());
#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IRotRutContext>(sp => sp.GetRequiredService<RotRutContext>());

        return services;
    }
}