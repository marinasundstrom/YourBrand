using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using TimeReport.Application.Common.Interfaces;
using TimeReport.Infrastructure.Persistence;
using TimeReport.Infrastructure.Services;

namespace TimeReport.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqlServer<TimeReportContext>(configuration.GetConnectionString("mssql", "TimeReport"));

        services.AddScoped<ITimeReportContext>(sp => sp.GetRequiredService<TimeReportContext>());

        services.AddScoped<IDomainEventService, DomainEventService>();

        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}