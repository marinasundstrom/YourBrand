using YourBrand.Warehouse.Application.Common.Interfaces;
using YourBrand.Warehouse.Infrastructure.Persistence;
using YourBrand.Warehouse.Infrastructure.Services;

namespace YourBrand.Warehouse.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddPersistence(configuration);

        services.AddScoped<IDomainEventService, DomainEventService>();

        services.AddTransient<IDateTime, DateTimeService>();

        return services;
    }
}