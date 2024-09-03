using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Catalog;

namespace YourBrand.StoreFront.API.Features.Products.Categories;

public static class Endpoints
{
    private static string? storeId = null;

    public static IEndpointRouteBuilder MapProductCategoriesEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi("ProductCategories");

        var productsGroup = versionedApi.MapGroup("/v{version:apiVersion}/productCategories")
            .WithTags("ProductCategories")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi()
            .RequireCors();

        productsGroup.MapGet("/", GetProductCategories)
            .WithName($"ProductCategories_{nameof(GetProductCategories)}");

        // {id} for Swagger
        productsGroup.MapGet("{*id}", GetProductCategoryById)
            .WithName($"ProductCategories_{nameof(GetProductCategoryById)}");

        productsGroup.MapGet("tree/{*idOrPath}", GetProductCategoryTree)
            .WithName($"ProductCategories_{nameof(GetProductCategoryTree)}");

        return app;
    }

    private static async Task<Results<Ok<PagedResult<ProductCategoryDto>>, NotFound>> GetProductCategories(int? page = 1, int? pageSize = 10, string? searchTerm = null, IConfiguration configuration = null, IProductCategoriesClient productCategoriesClient = default!, IStoresClient storesClient = default!, CancellationToken cancellationToken = default)
    {
        if (storeId is null)
        {
            var store = await storesClient.GetStoreByIdAsync(configuration["OrganizationId"]!, "my-store");
            storeId = store.Id;
        }

        var results = await productCategoriesClient.GetProductCategoriesAsync(configuration["OrganizationId"]!, storeId, null, false, false, page, pageSize, searchTerm, null, null, cancellationToken);
        return results is not null ? TypedResults.Ok(
                new PagedResult<ProductCategoryDto>(results.Items.Select(x => x.ToDto()), results.Total)
        ) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ProductCategoryDto>, NotFound>> GetProductCategoryById(string id, IConfiguration configuration, IProductCategoriesClient productCategoriesClient, CancellationToken cancellationToken = default)
    {
        var productCategory = await productCategoriesClient.GetProductCategoryByIdAsync(configuration["OrganizationId"]!, id, cancellationToken);
        return productCategory is not null ? TypedResults.Ok(productCategory.ToDto()) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ProductCategoryTreeRootDto>, NotFound>> GetProductCategoryTree(string? rootNodeIdOrPath, IConfiguration configuration, IProductCategoriesClient productCategoriesClient = default!, IStoresClient storesClient = default!, CancellationToken cancellationToken = default)
    {
        if (storeId is null)
        {
            var store = await storesClient.GetStoreByIdAsync(configuration["OrganizationId"]!, "my-store");
            storeId = store.Id;
        }

        var tree = await productCategoriesClient.GetProductCategoryTreeAsync(configuration["OrganizationId"]!, storeId, rootNodeIdOrPath, cancellationToken);
        return tree is not null ? TypedResults.Ok(tree.ToProductCategoryTreeRootDto()) : TypedResults.NotFound();
    }
}

public record class ProductCategoryDto(long Id, string Name, string Description, string Handle, string Path, ProductCategoryParent? Parent, long ProductsCount);

public record class ProductCategoryTreeRootDto(IEnumerable<ProductCategoryTreeNodeDto> Categories, long ProductsCount);

public record class ProductCategoryTreeNodeDto(long Id, string Name, string Handle, string Path, string? Description, ProductCategoryParent? Parent, IEnumerable<ProductCategoryTreeNodeDto> SubCategories, long ProductsCount, bool CanAddProducts);

public record class ProductCategoryParent(long Id, string Name, string Handle, string Path, ProductCategoryParent? Parent, long ProductsCount);


public static class Mapping
{
    public static ProductCategoryDto ToDto(this YourBrand.Catalog.ProductCategory productCategory)
    {
        return new(productCategory.Id, productCategory.Name, productCategory.Description ?? string.Empty, productCategory.Handle, productCategory.Path, null, productCategory.ProductsCount);
    }

    public static ProductCategoryTreeRootDto ToProductCategoryTreeRootDto(this YourBrand.Catalog.ProductCategoryTreeRoot productCategory)
    {
        return new ProductCategoryTreeRootDto(productCategory.Categories.Select(x => x.ToProductCategoryTreeNodeDto()), productCategory.ProductsCount);
    }

    public static ProductCategoryTreeNodeDto ToProductCategoryTreeNodeDto(this YourBrand.Catalog.ProductCategoryTreeNode productCategory)
    {
        return new(productCategory.Id, productCategory.Name, productCategory.Handle, productCategory.Path, productCategory.Description, productCategory.Parent?.ToParentDto(), productCategory.SubCategories.Select(x => x.ToProductCategoryTreeNodeDto()), productCategory.ProductsCount, productCategory.CanAddProducts);
    }

    public static ProductCategoryParent ToParentDto(this YourBrand.Catalog.ParentProductCategoryTreeNode productCategory)
    {
        return new(productCategory.Id, productCategory.Name, productCategory.Handle, productCategory.Path, productCategory.Parent?.ToParentDto(), productCategory.ProductsCount);
    }

    public static ProductCategoryParent ToParentDto2(this YourBrand.Catalog.ParentProductCategory productCategory)
    {
        return new(productCategory.Id, productCategory.Name, productCategory.Handle, productCategory.Path, productCategory.Parent?.ToParentDto2(), productCategory.ProductsCount);
    }

    public static ProductCategoryParent ToParentDto3(this YourBrand.Catalog.ProductCategory2 productCategory)
    {
        return new(productCategory.Id, productCategory.Name, productCategory.Handle, productCategory.Path, productCategory.Parent?.ToParentDto2(), productCategory.ProductsCount);
    }
}