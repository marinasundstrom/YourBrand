using System;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using YourBrand.TimeReport.Application.Common.Interfaces;
using YourBrand.TimeReport.Infrastructure.Persistence;
using YourBrand.TimeReport.Infrastructure.Services;

namespace YourBrand.TimeReport.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqlServer<TimeReportContext>(
            configuration.GetConnectionString("mssql", "TimeReport") ?? configuration.GetConnectionString("DefaultConnection"),
            options => options.EnableRetryOnFailure());

        services.AddScoped<ITimeReportContext>(sp => sp.GetRequiredService<TimeReportContext>());

        services.AddScoped<IDomainEventService, DomainEventService>();

        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}