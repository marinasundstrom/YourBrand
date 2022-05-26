using YourBrand.Transactions.Application.Common.Interfaces;
using YourBrand.Transactions.Infrastructure.Persistence;
using YourBrand.Transactions.Infrastructure.Services;

namespace YourBrand.Transactions.Infrastructure;

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