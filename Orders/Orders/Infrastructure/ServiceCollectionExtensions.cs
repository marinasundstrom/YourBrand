using YourBrand.Orders.Application.Common.Interfaces;
using YourBrand.Orders.Infrastructure.Persistence;
using YourBrand.Orders.Infrastructure.Services;

namespace YourBrand.Orders.Infrastructure;

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