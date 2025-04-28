using YourBrand.Catalog.Features.ProductManagement.Products.Images;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public static class ServicesExtensions
{
    public static IServiceCollection AddProductsServices(this IServiceCollection services)
    {
        services.AddScoped<ProductPricingService>();
        
        services.AddScoped<IProductImageUploader, ProductImageUploader>();

        return services;
    }
}