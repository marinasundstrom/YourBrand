using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.Products;

public static class ServiceExtensions
{
    public static IServiceCollection AddProductsServices(this IServiceCollection services)
    {
        services.AddScoped<IProductsService, ProductsService>();

        return services;
    }
}