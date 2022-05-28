using YourBrand.Payments.Application.Common.Interfaces;
using YourBrand.Payments.Infrastructure.Persistence;
using YourBrand.Payments.Infrastructure.Services;

namespace YourBrand.Payments.Infrastructure;

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