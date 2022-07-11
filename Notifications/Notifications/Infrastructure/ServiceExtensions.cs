using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.Notifications.Application.Common.Interfaces;
using YourBrand.Notifications.Infrastructure.Persistence;
using YourBrand.Notifications.Infrastructure.Persistence.Interceptors;
using YourBrand.Notifications.Infrastructure.Services;

namespace YourBrand.Notifications.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqlServer<WorkerContext>(
            configuration.GetConnectionString("mssql", "Worker") ?? configuration.GetConnectionString("DefaultConnection"),
        options => options.EnableRetryOnFailure());

        services.AddScoped<IWorkerContext>(sp => sp.GetRequiredService<WorkerContext>());

        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddScoped<IDomainEventService, DomainEventService>();

        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}