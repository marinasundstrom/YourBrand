using Invoices.Application.Common.Interfaces;
using Invoices.Infrastructure.Persistence;
using Invoices.Infrastructure.Services;

namespace Invoices.Infrastructure;

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