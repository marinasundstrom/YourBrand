using System;

using YourBrand.Showroom.Application.Common.Interfaces;
using YourBrand.Showroom.Infrastructure.Persistence;
using YourBrand.Showroom.Infrastructure.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Showroom.Infrastructure;

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