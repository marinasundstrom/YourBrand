using Microsoft.Extensions.DependencyInjection;

namespace BlazorApp.ProductCategories;

public static class ServiceExtensions
{
    public static IServiceCollection AddProductCategoriesServices(this IServiceCollection services)
    {
        services.AddScoped<IProductCategoryService, ProductCategoryService>();

        return services;
    }
}