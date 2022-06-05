using YourBrand.Documents.Application.Common.Interfaces;
using YourBrand.Documents.Infrastructure.Persistence;
using YourBrand.Documents.Infrastructure.Services;

namespace YourBrand.Documents.Infrastructure;

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