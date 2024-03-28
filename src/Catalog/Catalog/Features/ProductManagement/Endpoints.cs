using YourBrand.Catalog.Features.ProductManagement.Attributes;
using YourBrand.Catalog.Features.ProductManagement.Options;
using YourBrand.Catalog.Features.ProductManagement.ProductCategories;
using YourBrand.Catalog.Features.ProductManagement.Products;

namespace YourBrand.Catalog.Features.ProductManagement;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapProductManagementEndpoints(this IEndpointRouteBuilder app)
    {
        app
        .MapProductsEndpoints()
        .MapProductCategoriesEndpoints()
        .MapAttributesEndpoints()
        .MapOptionsEndpoints();

        return app;
    }
}