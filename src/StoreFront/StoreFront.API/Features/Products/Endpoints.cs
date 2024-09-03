using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Catalog;
using YourBrand.StoreFront.API.Features.Products.Categories;

namespace YourBrand.StoreFront.API.Features.Products;

public static class Endpoints
{
    private static string? storeId = null;

    public static IEndpointRouteBuilder MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapProductCategoriesEndpoints();

        var versionedApi = app.NewVersionedApi("Products");

        var productsGroup = versionedApi.MapGroup("/v{version:apiVersion}/products")
            .WithTags("Products")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi()
            .RequireCors();

        productsGroup.MapGet("/", GetProducts)
            .WithName($"Products_{nameof(GetProducts)}")
            .CacheOutput(OutputCachePolicyNames.GetProducts);

        productsGroup.MapGet("/{productIdOrHandle}", GetProductById)
            .WithName($"Products_{nameof(GetProductById)}")
            .CacheOutput(OutputCachePolicyNames.GetProductById);

        productsGroup.MapPost("/{productIdOrHandle}/findVariant", FindProductVariantByAttributes)
            .WithName($"Products_{nameof(FindProductVariantByAttributes)}");

        productsGroup.MapPost("/{productIdOrHandle}/find", FindProductVariantsByAttributes)
            .WithName($"Products_{nameof(FindProductVariantsByAttributes)}");

        productsGroup.MapGet("/{productIdOrHandle}/variants", GetProductVariants)
            .WithName($"Products_{nameof(GetProductVariants)}");

        productsGroup.MapPost("/{productIdOrHandle}/attributes/{attributeId}/availableValuesForVariant", GetAvailableVariantAttributeValues)
            .WithName($"Products_{nameof(GetAvailableVariantAttributeValues)}");

        return app;
    }

    private static async Task<Results<Ok<PagedResult<Product>>, NotFound>> GetProducts(string? brandIdOrHandle, int? page = 1, int? pageSize = 10, string? searchTerm = null, string? categoryPath = null, IConfiguration configuration = null, IProductsClient productsClient = default!, IStoresClient storesClient = default!, CancellationToken cancellationToken = default)
    {
        if (storeId is null)
        {
            var store = await storesClient.GetStoreByIdAsync(configuration["OrganizationId"]!, "my-store");
            storeId = store.Id;
        }

        var results = await productsClient.GetProductsAsync(configuration["OrganizationId"]!, storeId, brandIdOrHandle, false, true, searchTerm, categoryPath, page, pageSize, null, null, cancellationToken);
        return results is not null ? TypedResults.Ok(
                new PagedResult<Product>(results.Items.Select(x => x.Map()), results.Total)
        ) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<Product>, NotFound>> GetProductById(string productIdOrHandle, IConfiguration configuration, IProductsClient productsClient = default!, CancellationToken cancellationToken = default)
    {
        var product = await productsClient.GetProductByIdAsync(configuration["OrganizationId"]!, productIdOrHandle, cancellationToken);
        return product is not null ? TypedResults.Ok(product.Map()) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<Product>, NotFound>> FindProductVariantByAttributes(string productIdOrHandle, Dictionary<string, string?> selectedAttributeValues, IConfiguration configuration, IProductsClient productsClient = default!, CancellationToken cancellationToken = default)
    {
        var product = await productsClient.FindVariantByAttributeValuesAsync(configuration["OrganizationId"]!, productIdOrHandle, selectedAttributeValues, cancellationToken);
        return product is not null ? TypedResults.Ok(product.Map()) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<IEnumerable<Product>>, NotFound>> FindProductVariantsByAttributes(string productIdOrHandle, Dictionary<string, string?> selectedAttributeValues, IConfiguration configuration, IProductsClient productsClient = default!, CancellationToken cancellationToken = default)
    {
        var products = await productsClient.FindsVariantsByAttributeValuesAsync(configuration["OrganizationId"]!, productIdOrHandle, selectedAttributeValues, cancellationToken);
        return products is not null ? TypedResults.Ok(products.Select(x => x.Map())) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<PagedResult<Product>>, NotFound>> GetProductVariants(string productIdOrHandle, int page = 10, int pageSize = 10, string? searchTerm = null, IConfiguration configuration = null, IProductsClient productsClient = default!, CancellationToken cancellationToken = default)
    {
        var results = await productsClient.GetVariantsAsync(configuration["OrganizationId"]!, productIdOrHandle, page, pageSize, searchTerm, null, null, cancellationToken);
        return results is not null ? TypedResults.Ok(
                new PagedResult<Product>(results.Items.Select(x => x.Map()), results.Total)
        ) : TypedResults.NotFound();
    }

    public static async Task<Results<Ok<IEnumerable<AttributeValue>>, BadRequest>> GetAvailableVariantAttributeValues(string productIdOrHandle, string attributeId, Dictionary<string, string?> selectedAttributeValues, IConfiguration configuration, IProductsClient productsClient = default!, CancellationToken cancellationToken = default!)
    {
        var results = await productsClient.GetAvailableVariantAttributeValuesAsync(configuration["OrganizationId"]!, productIdOrHandle, attributeId, selectedAttributeValues, cancellationToken);
        return results is not null ? TypedResults.Ok(results.Select(x => x.Map())) : TypedResults.BadRequest();

    }
}