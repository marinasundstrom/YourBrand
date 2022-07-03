using YourBrand.Products.Application.Common.Interfaces;
using YourBrand.Products.Infrastructure.Persistence;
using YourBrand.Products.Infrastructure.Services;

namespace YourBrand.Products.Infrastructure;

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