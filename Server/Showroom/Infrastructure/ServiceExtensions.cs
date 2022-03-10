using System;

using Skynet.Showroom.Application.Common.Interfaces;
using Skynet.Showroom.Infrastructure.Persistence;
using Skynet.Showroom.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Skynet.Showroom.Infrastructure;

public static class ServiceExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        return services;
    }

    private static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSqlServer<ShowroomContext>(configuration.GetConnectionString("mssql", "Showroom") ?? configuration.GetConnectionString("DefaultConnection"));

        services.AddScoped<IShowroomContext>(sp => sp.GetRequiredService<ShowroomContext>());

        services.AddScoped<IDomainEventService, DomainEventService>();

        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}