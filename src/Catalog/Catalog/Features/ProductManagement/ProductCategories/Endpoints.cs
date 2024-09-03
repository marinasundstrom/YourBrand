using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Catalog.Model;

namespace YourBrand.Catalog.Features.ProductManagement.ProductCategories;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapProductCategoriesEndpoints(this IEndpointRouteBuilder app)
    {
        string GetProductCategoriesExpire20 = nameof(GetProductCategoriesExpire20);

        var versionedApi = app.NewVersionedApi("ProductCategories");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/productCategories")
            .WithTags("ProductCategories")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/", GetProductCategories)
            .WithName($"ProductCategories_{nameof(GetProductCategories)}");

        /*

        app.MapGet("/{idOrPath}", GetProductCategoryById)
            .WithName($"ProductCategories_{nameof(GetProductCategoryById)}");

        */

        group.MapGet("tree/{*idOrPath}", GetProductCategoryTree)
            .WithName($"ProductCategories_{nameof(GetProductCategoryTree)}");

        group.MapPost("/", CreateProductCategory)
            .WithName($"ProductCategories_{nameof(CreateProductCategory)}");

        group.MapPut("{*idOrPath}", UpdateProductCategoryDetails)
            .WithName($"ProductCategories_{nameof(UpdateProductCategoryDetails)}");

        group.MapDelete("{*idOrPath}", DeleteProductCategory)
            .WithName($"ProductCategories_{nameof(DeleteProductCategory)}");

        return app;
    }

    private static async Task<Ok<PagedResult<ProductCategory>>> GetProductCategories(string organizationId, string? storeId = null, long? parentGroupId = null, bool includeWithUnlistedProducts = false, bool includeHidden = false,
        int page = 0, int pageSize = 10, string? searchString = null, string? sortBy = null, SortDirection? sortDirection = null,
            IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var pagedResult = await mediator.Send(new GetProductCategories(organizationId, storeId, parentGroupId, includeWithUnlistedProducts, includeHidden, page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
        return TypedResults.Ok(pagedResult);
    }

    /*

    private static async Task<Results<Ok<ProductCategory>, NotFound>> GetProductCategoryById(string idOrPath,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductCategoryById(idOrPath), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.GetValue()) : TypedResults.NotFound();
    }

    */

    private static async Task<Results<Ok<ProductCategoryTreeRootDto>, BadRequest>> GetProductCategoryTree(string organizationId,
        string? storeId, string? rootNodeIdOrPath, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductCategoryTree(organizationId, storeId, rootNodeIdOrPath), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.GetValue()) : TypedResults.BadRequest();
    }

    private static async Task<Results<Ok<ProductCategory>, BadRequest, ProblemHttpResult>> CreateProductCategory(string organizationId, CreateProductCategoryRequest request,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateProductCategory(organizationId, request.Name, request.Description, request.ParentCategoryId, request.Handle, request.StoreId), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.GetValue()) : TypedResults.BadRequest();
    }

    private static async Task<Results<Ok<ProductCategory>, NotFound>> UpdateProductCategoryDetails(string organizationId, string idOrPath, UpdateProductCategoryDetailsRequest request,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateProductCategoryDetails(organizationId, idOrPath, request.Name, request.Description), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.GetValue()) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound>> DeleteProductCategory(string organizationId, string idOrPath,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteProductCategory(organizationId, idOrPath), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.NotFound();
    }
}

public sealed record CreateProductCategoryRequest(string Name, string Description, long? ParentCategoryId, string Handle, string? StoreId);

public sealed record UpdateProductCategoryDetailsRequest(string Name, string Description);

public static class Mapping
{
    public static ProductCategory ToDto(this Domain.Entities.ProductCategory productCategory)
    {
        return new(productCategory.Id, productCategory.Name, productCategory.Description, productCategory.Parent?.ToParentProductCategory(), productCategory.CanAddProducts, productCategory.ProductsCount, productCategory.Handle, productCategory.Path);
    }

    public static ParentProductCategory ToParentProductCategory(this Domain.Entities.ProductCategory productCategory)
    {
        return new(productCategory.Id, productCategory.Name, productCategory.Description, productCategory.Handle, productCategory.Path, productCategory.Parent?.ToParentProductCategory(), productCategory.ProductsCount);
    }

    public static ProductCategory2 ToProductCategory2(this Domain.Entities.ProductCategory productCategory)
    {
        return new(productCategory.Id, productCategory.Name, productCategory.Description, productCategory.Handle, productCategory.Path, productCategory.Parent?.ToParentProductCategory(), productCategory.ProductsCount);
    }

    public static ProductCategoryTreeNodeDto ToProductCategoryTreeNodeDto(this Domain.Entities.ProductCategory productCategory)
    {
        return new(productCategory.Id, productCategory.Name, productCategory.Handle, productCategory.Path, productCategory.Description, productCategory.Parent?.ToParentProductCategoryTreeNodeDto(), productCategory.SubCategories.Select(x => x.ToProductCategoryTreeNodeDto()), productCategory.ProductsCount, productCategory.CanAddProducts);
    }

    public static ParentProductCategoryTreeNodeDto ToParentProductCategoryTreeNodeDto(this Domain.Entities.ProductCategory productCategory)
    {
        return new(productCategory.Id, productCategory.Name, productCategory.Handle, productCategory.Path, productCategory.Parent?.ToParentProductCategoryTreeNodeDto(), productCategory.ProductsCount);
    }
}

public sealed record ProductCategory(
    long Id,
    string Name,
    string? Description,
    ParentProductCategory? Parent,
    bool CanAddProducts,
    long ProductsCount,
    string Handle,
    string Path
);


public sealed record ParentProductCategory(
    long Id,
    string Name,
    string? Description,
    string Handle,
    string Path,
    ParentProductCategory? Parent,
    long ProductsCount
);

public sealed record ProductCategory2(
    long Id,
    string Name,
    string? Description,
    string Handle,
    string Path,
    ParentProductCategory? Parent,
    long ProductsCount
);

public record class ProductCategoryTreeRootDto(
    IEnumerable<ProductCategoryTreeNodeDto> Categories,
    long ProductsCount);

public record class ProductCategoryTreeNodeDto(
    long Id,
    string Name,
    string Handle,
    string Path,
    string? Description,
    ParentProductCategoryTreeNodeDto? Parent,
    IEnumerable<ProductCategoryTreeNodeDto> SubCategories,
    long ProductsCount,
    bool CanAddProducts);

public record class ParentProductCategoryTreeNodeDto(
    long Id,
    string Name,
    string Handle,
    string Path,
    ParentProductCategoryTreeNodeDto? Parent,
    long ProductsCount);