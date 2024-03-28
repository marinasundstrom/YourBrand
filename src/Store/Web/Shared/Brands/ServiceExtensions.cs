using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Brands;

public static class ServiceExtensions
{
    public static IServiceCollection AddBrandsServices(this IServiceCollection services)
    {
        services.AddScoped<IBrandsService, BrandsService>();

        return services;
    }
}