using MediatR;

using YourBrand.Catalog.Application.Products.Variants;

namespace YourBrand.Catalog.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(ServiceCollectionExtensions)));

        services.AddScoped<ProductVariantsService>();

        return services;
    }
}