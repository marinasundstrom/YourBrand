using YourBrand.Invoices.Application.Common.Interfaces;
using YourBrand.Invoices.Infrastructure.Persistence;
using YourBrand.Invoices.Infrastructure.Services;

namespace YourBrand.Invoices.Infrastructure;

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