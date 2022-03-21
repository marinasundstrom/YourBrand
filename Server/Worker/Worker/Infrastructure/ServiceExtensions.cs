using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Worker.Application.Common.Interfaces;
using Worker.Infrastructure.Persistence;
using Worker.Infrastructure.Services;

namespace Worker.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqlServer<WorkerContext>(configuration.GetConnectionString("mssql", "Worker") ?? configuration.GetConnectionString("DefaultConnection"));

        services.AddScoped<IWorkerContext>(sp => sp.GetRequiredService<WorkerContext>());

        services.AddScoped<IDomainEventService, DomainEventService>();

        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}