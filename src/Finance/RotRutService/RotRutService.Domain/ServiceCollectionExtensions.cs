using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.RotRutService.Domain;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddScoped<RotRutCaseFactory>();

        return services;
    }
}