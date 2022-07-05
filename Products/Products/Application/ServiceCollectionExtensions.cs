using MediatR;

using YourBrand.Products.Application.Products.Variants;

namespace YourBrand.Products.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(ServiceCollectionExtensions));

        services.AddScoped<ProductVariantsService>();

        return services;
    }
}