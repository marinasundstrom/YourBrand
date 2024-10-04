using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Meetings.Infrastructure.Persistence.Interceptors;
using YourBrand.Meetings.Infrastructure.Persistence.Repositories;

namespace YourBrand.Meetings.Infrastructure.Persistence;

public static class ServiceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = Infrastructure.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, "Meetings")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString!, o => o.EnableRetryOnFailure());

            options.AddInterceptors(
                sp.GetRequiredService<OutboxSaveChangesInterceptor>(),
                sp.GetRequiredService<IAuditableSaveChangesInterceptor>());

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<IApplicationDbContext>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IAuditableSaveChangesInterceptor>();
        services.AddScoped<OutboxSaveChangesInterceptor>();

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