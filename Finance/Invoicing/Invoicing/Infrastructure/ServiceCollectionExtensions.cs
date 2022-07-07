using YourBrand.Invoicing.Application.Common.Interfaces;
using YourBrand.Invoicing.Infrastructure.Persistence;
using YourBrand.Invoicing.Infrastructure.Services;

namespace YourBrand.Invoicing.Infrastructure;

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