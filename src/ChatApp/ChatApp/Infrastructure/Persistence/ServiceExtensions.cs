﻿using Microsoft.EntityFrameworkCore;

using YourBrand.ChatApp.Domain;
using YourBrand.ChatApp.Infrastructure.Persistence.Repositories;
using YourBrand.ChatApp.Persistence.Interceptors;
using YourBrand.Domain.Persistence.Interceptors;

namespace YourBrand.ChatApp.Infrastructure.Persistence;

public static class ServiceExtensions
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        const string ConnectionStringKey = "mssql";

        var connectionString = Infrastructure.ConfigurationExtensions.GetConnectionString(configuration, ConnectionStringKey, "ChatApp")
            ?? configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.UseSqlServer(connectionString!, o => o.EnableRetryOnFailure());

            options.AddInterceptors(
                sp.GetRequiredService<OutboxSaveChangesInterceptor>(),
                sp.GetRequiredService<AuditableEntitySaveChangesInterceptor>());

#if DEBUG
            options
                .EnableSensitiveDataLogging();
#endif
        });

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddScoped<OutboxSaveChangesInterceptor>();

        RegisterRepositories(services);

        return services;
    }

    private static void RegisterRepositories(IServiceCollection services)
    {
        // TODO: Automate this

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IChannelRepository, ChannelRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();

        services.Decorate<IUserRepository, CachedUserRepository>();
    }
}