using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourCompany.TimeReport.Application.Common.Interfaces;
using YourCompany.TimeReport.Infrastructure.Persistence;
using YourCompany.TimeReport.Infrastructure.Services;

namespace YourCompany.TimeReport.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqlServer<TimeReportContext>(configuration.GetConnectionString("mssql", "TimeReport") ?? configuration.GetConnectionString("DefaultConnection"));

        services.AddScoped<ITimeReportContext>(sp => sp.GetRequiredService<TimeReportContext>());

        services.AddScoped<IDomainEventService, DomainEventService>();

        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}