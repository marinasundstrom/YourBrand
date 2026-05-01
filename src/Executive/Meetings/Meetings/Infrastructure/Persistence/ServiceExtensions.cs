using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Tenancy;
using YourBrand.Meetings.Infrastructure.Persistence.Interceptors;
using YourBrand.Meetings.Infrastructure.Persistence.Repositories;

namespace YourBrand.Meetings.Infrastructure.Persistence;

public static class ServiceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";
        const string DatabaseName = "Meetings";

        var connectionString = Infrastructure.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, DatabaseName)
            ?? configuration.GetConnectionString("DefaultConnection");
        var databasePerTenant = configuration.IsDatabasePerTenantEnabled("Meetings");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString!, o => o.EnableRetryOnFailure());

            options.AddInterceptors(
                sp.GetRequiredService<OutboxSaveChangesInterceptor>(),
                sp.GetRequiredService<IAuditableSaveChangesInterceptor>());

            options.UseTenantDatabasePerTenant<ApplicationDbContext>(sp, databasePerTenant);

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IAuditableSaveChangesInterceptor>();
        services.AddScoped<OutboxSaveChangesInterceptor>();
        services.AddTenantDatabasePerTenant(databasePerTenant, builder =>
        {
            builder.ForContext<ApplicationDbContext>()
                .WithDatabasePrefix(DatabaseName);
        });

        RegisterRepositories(services);

        return services;
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        // TODO: Automate this

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IMeetingRepository, MeetingRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();
    }
}
