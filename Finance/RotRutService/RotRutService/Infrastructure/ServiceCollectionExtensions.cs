using YourBrand.RotRutService.Application.Common.Interfaces;
using YourBrand.RotRutService.Infrastructure.Persistence;
using YourBrand.RotRutService.Infrastructure.Services;

namespace YourBrand.RotRutService.Infrastructure;

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