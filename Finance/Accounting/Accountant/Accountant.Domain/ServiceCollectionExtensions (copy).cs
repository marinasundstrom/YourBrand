using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Accountant.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<EntriesFactory>();

        return services;
    }
}