using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Auditability;
using YourBrand.Domain.Persistence;
using YourBrand.Tenancy;

namespace YourBrand.Analytics.Infrastructure.Persistence;

public static class ServiceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";
        const string DatabaseName = "Analytics";

        var connectionString = Infrastructure.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, DatabaseName)
            ?? configuration.GetConnectionString("DefaultConnection");
        var databasePerTenant = configuration.IsDatabasePerTenantEnabled("Analytics");

        services.AddDomainPersistence<ApplicationDbContext>(configuration);

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            options.UseSqlServer(connectionString!, o => o.EnableRetryOnFailure());

            options
                .UseDomainInterceptors(serviceProvider)
                .UseTenancyInterceptor(serviceProvider)
                .UseTenantDatabasePerTenant<ApplicationDbContext>(serviceProvider, databasePerTenant)
                .UseAuditabilityInterceptor(serviceProvider)
                .UseSoftDeleteInterceptor(serviceProvider);

#if DEBUG
            options
                .EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddTenancyInterceptor();
        services.AddTenantDatabasePerTenant(databasePerTenant, builder =>
        {
            builder.ForContext<ApplicationDbContext>()
                .WithDatabasePrefix(DatabaseName);
        });
        services.AddAuditabilityInterceptor();
        services.AddSoftDeleteInterceptor();

        return services;
    }
}
