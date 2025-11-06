using YourBrand.Catalog.Features.ProductManagement.Import;
using YourBrand.Catalog.Features.ProductManagement.Products.Images;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public static class ServicesExtensions
{
    public static IServiceCollection AddProductsServices(this IServiceCollection services)
    {
        services.AddScoped<ProductPricingService>();
        services.AddScoped<ProductOptionValidationService>();

        services.AddScoped<IProductImageUploader, ProductImageUploader>();
        services.AddScoped<IProductImportArchiveManager, FileSystemProductImportArchiveManager>();

        return services;
    }
}