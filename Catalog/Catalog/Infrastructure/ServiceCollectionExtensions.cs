using YourBrand.Catalog.Application.Common.Interfaces;
using YourBrand.Catalog.Infrastructure.Persistence;
using YourBrand.Catalog.Infrastructure.Services;

namespace YourBrand.Catalog.Infrastructure;

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