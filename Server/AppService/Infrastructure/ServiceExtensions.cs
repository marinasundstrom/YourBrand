using System;

using Catalog.Application.Common.Interfaces;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqlServer<CatalogContext>(configuration.GetConnectionString("mssql", "Catalog"));

        services.AddScoped<ICatalogContext>(sp => sp.GetRequiredService<CatalogContext>());

        services.AddScoped<IDomainEventService, DomainEventService>();

        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}