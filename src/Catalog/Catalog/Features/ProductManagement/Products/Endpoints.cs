using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Catalog.Features.Brands;
using YourBrand.Catalog.Features.ProductManagement.Attributes;
using YourBrand.Catalog.Features.ProductManagement.Import;
using YourBrand.Catalog.Features.ProductManagement.Options;
using YourBrand.Catalog.Features.ProductManagement.ProductCategories;
using YourBrand.Catalog.Features.ProductManagement.Products.Attributes;
using YourBrand.Catalog.Features.ProductManagement.Products.Images;
using YourBrand.Catalog.Features.ProductManagement.Products.Options;
using YourBrand.Catalog.Features.ProductManagement.SubscriptionPlans.SubscriptionPlans;
using YourBrand.Catalog.Features.Stores;
using YourBrand.Catalog.Model;
using YourBrand.Extensions;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public static partial class Endpoints
{
    public static IEndpointRouteBuilder MapProductsEndpoints(this IEndpointRouteBuilder app)
    {
        MapProductVariantsEndpoints(app);

        app.MapProductAttributesEndpoints()
            .MapProductOptionsEndpoints()
            .MapProductSubscriptionPlansEndpoints();

        var versionedApi = app.NewVersionedApi("Products");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/products")
            .WithTags("Products")
            .RequireRateLimiting(RateLimiterPolicyNames.FixedRateLimiter)
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/", GetProducts)
            .WithName($"Products_{nameof(GetProducts)}")
            .CacheOutput(OutputCachePolicyNames.GetProducts);

        group.MapGet("/ByIds", GetProductsByIds)
            .WithName($"Products_{nameof(GetProductsByIds)}");

        group.MapGet("/{idOrHandle}", GetProductById)
            .WithName($"Products_{nameof(GetProductById)}")
            .CacheOutput(OutputCachePolicyNames.GetProductById);

        group.MapPost("/{idOrHandle}/price", CalculateProductPrice)
            .WithName($"Products_{nameof(CalculateProductPrice)}");

        group.MapPost("/", CreateProduct)
            .AddEndpointFilter<ValidationFilter<CreateProductRequest>>()
            .WithName($"Products_{nameof(CreateProduct)}");

        group.MapPut("/{idOrHandle}", UpdateProductDetails)
            .AddEndpointFilter<ValidationFilter<UpdateProductDetailsRequest>>()
            .WithName($"Products_{nameof(UpdateProductDetails)}");

        group.MapPut("/{idOrHandle}/price", UpdateProductPrice)
            .WithName($"Products_{nameof(UpdateProductPrice)}");

        group.MapPut("/{idOrHandle}/vatRate", UpdateProductVatRate)
            .WithName($"Products_{nameof(UpdateProductVatRate)}");

        group.MapPost("/{idOrHandle}/price/discount", SetProductDiscountPrice)
            .WithName($"Products_{nameof(SetProductDiscountPrice)}");

        group.MapPost("/{idOrHandle}/price/restore", RestoreProductRegularPrice)
            .WithName($"Products_{nameof(RestoreProductRegularPrice)}");

        group.MapPost("/{idOrHandle}/images", UploadProductImage)
            .WithName($"Products_{nameof(UploadProductImage)}")
            .DisableAntiforgery();

        group.MapPut("/{idOrHandle}/images/{productImageId}", UpdateProductImage)
            .WithName($"Products_{nameof(UpdateProductImage)}");

        group.MapPut("/{idOrHandle}/images/{productImageId}/setMainImage", SetMainProductImage)
            .WithName($"Products_{nameof(SetMainProductImage)}");

        group.MapDelete("/{idOrHandle}/images/{productImageId}", DeleteProductImage)
            .WithName($"Products_{nameof(DeleteProductImage)}");

        group.MapPut("/{idOrHandle}/handle", UpdateProductHandle)
            .AddEndpointFilter<ValidationFilter<UpdateProductHandleRequest>>()
            .WithName($"Products_{nameof(UpdateProductHandle)}");

        group.MapPut("/{idOrHandle}/sku", UpdateProductSku)
            .AddEndpointFilter<ValidationFilter<UpdateProductSkuRequest>>()
            .WithName($"Products_{nameof(UpdateProductSku)}");

        group.MapPut("/{idOrHandle}/listingState", UpdateProductListingState)
            .AddEndpointFilter<ValidationFilter<UpdateProductListingStateRequest>>()
            .WithName($"Products_{nameof(UpdateProductListingState)}");

        group.MapPut("/{idOrHandle}/category", UpdateProductCategory)
            .AddEndpointFilter<ValidationFilter<CreateProductCategoryRequest>>()
            .WithName($"Products_{nameof(UpdateProductCategory)}");

        group.MapPut("/{idOrHandle}/brand", UpdateProductBrand)
            //.AddEndpointFilter<ValidationFilter<CreateProductCategoryRequest>>()
            .WithName($"Products_{nameof(UpdateProductBrand)}");

        group.MapDelete("/{idOrHandle}", DeleteProduct)
            .WithName($"Products_{nameof(DeleteProduct)}");

        group.MapPost("/import", ImportProducts)
            .WithName($"Products_{nameof(ImportProducts)}")
            .Produces<ProductImportResult>(StatusCodes.Status200OK)
            .DisableAntiforgery();

        return app;
    }

    private static async Task<Ok<PagedResult<ProductDto>>> GetProducts(string organizationId, string? storeId = null, string? brandIdOrHandle = null, bool includeUnlisted = false, bool groupProducts = true, string? searchTerm = null, string? categoryPathOrId = null,
        int page = 1, int pageSize = 10, string? sortBy = null, SortDirection? sortDirection = null, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var pagedResult = await mediator.Send(new GetProducts(organizationId, storeId, brandIdOrHandle, includeUnlisted, groupProducts, categoryPathOrId, searchTerm, page, pageSize, sortBy, sortDirection), cancellationToken);
        return TypedResults.Ok(pagedResult);
    }

    private static async Task<Ok<IEnumerable<ProductDto>>> GetProductsByIds(string organizationId, long[] ids, string? storeId = null, string? brandIdOrHandle = null, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var pagedResult = await mediator.Send(new GetProductsByIds(organizationId, ids, storeId, brandIdOrHandle), cancellationToken);
        return TypedResults.Ok(pagedResult);
    }

    private static async Task<Results<Ok<ProductDto>, NotFound>> GetProductById(string organizationId, string idOrHandle,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductById(organizationId, idOrHandle), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.GetValue()) : TypedResults.NotFound();
    }
    
    private static async Task<Results<Ok<ProductPriceResult>, NotFound>> CalculateProductPrice(string organizationId, string idOrHandle, CalculateProductPriceRequest request, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CalculateProductPrice(organizationId, idOrHandle, request.OptionValues, request.SubscriptionPlanId), cancellationToken);

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<ProductDto>, BadRequest, ProblemHttpResult>> CreateProduct(string organizationId, CreateProductRequest request,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateProduct(organizationId, request.Name, request.StoreId, request.Description, request.CategoryId, request.IsGroupedProduct, request.Price, request.VatRateId, request.Handle), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.GetValue()) : TypedResults.BadRequest();
    }

    private static async Task<Results<Ok, NotFound>> UpdateProductDetails(string organizationId, string idOrHandle, UpdateProductDetailsRequest request,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateProductDetails(organizationId, idOrHandle, request.Name, request.Description), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound>> UpdateProductPrice(string organizationId, string idOrHandle, UpdateProductPriceRequest request,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateProductPrice(organizationId, idOrHandle, request.Price), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound>> UpdateProductVatRate(string organizationId, string idOrHandle, UpdateProductVatRateRequest request,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateProductVatRate(organizationId, idOrHandle, request.VatRateId), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound>> SetProductDiscountPrice(string organizationId, string idOrHandle, SetProductDiscountPriceRequest request,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new SetProductDiscountPrice(organizationId, idOrHandle, request.DiscountPrice), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound>> RestoreProductRegularPrice(string organizationId, string idOrHandle, RestoreProductRegularPriceReguest request,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new RestoreProductRegularPrice(organizationId, idOrHandle), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ProductImageDto>, NotFound>> UploadProductImage(string organizationId, string idOrHandle, bool setMainImage = false, IFormFile file = default!,
        IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new UploadProductImage(organizationId, idOrHandle, file.OpenReadStream(), file.FileName, file.ContentType, setMainImage), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.GetValue()) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ProductImageDto>, NotFound>> UpdateProductImage(string organizationId, string idOrHandle, string productImageId, UpdateProductImageData data,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateProductImage(organizationId, idOrHandle, productImageId, data.Title, data.Text), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.GetValue()) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ProductImageDto>, NotFound>> SetMainProductImage(string organizationId, string idOrHandle, string productImageId, SetMainProductImageData data,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new SetMainProductImage(organizationId, idOrHandle, productImageId), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.GetValue()) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ProductImageDto>, NotFound>> DeleteProductImage(string organizationId, string idOrHandle, string productImageId,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteProductImage(organizationId, idOrHandle, productImageId), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.GetValue()) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound, ProblemHttpResult>> UpdateProductHandle(string organizationId, string idOrHandle, UpdateProductHandleRequest request,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateProductHandle(organizationId, idOrHandle, request.Handle), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound, ProblemHttpResult>> UpdateProductSku(string organizationId, string idOrHandle, UpdateProductSkuRequest request,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateProductSku(organizationId, idOrHandle, request.Sku), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound, ProblemHttpResult>> UpdateProductListingState(string organizationId, string idOrHandle, UpdateProductListingStateRequest request,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateProductListingState(organizationId, idOrHandle, request.ListingState), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.NotFound();
    }


    private static async Task<Results<Ok, NotFound, ProblemHttpResult>> UpdateProductCategory(string organizationId, string idOrHandle, UpdateProductCategoryRequest request,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateProductCategory(organizationId, idOrHandle, request.ProductCategoryId), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound, ProblemHttpResult>> UpdateProductBrand(string organizationId, string idOrHandle, UpdateProductBrandRequest request,
        IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateProductBrand(organizationId, idOrHandle, request.BrandId), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound>> DeleteProduct(string organizationId, string idOrHandle,
    IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteProduct(organizationId, idOrHandle), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ProductImportResult>, NotFound>> ImportProducts(string organizationId, IFormFile file,
   IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new ImportProducts(organizationId, file.OpenReadStream()), cancellationToken);

        return result.IsSuccess ? TypedResults.Ok(result.GetValue()) : TypedResults.NotFound();
    }
}

public sealed record CreateProductRequest(string Name, string StoreId, string Description, long CategoryId, bool IsGroupedProduct, decimal Price, int? VatRateId, string Handle)
{
    public class CreateProductRequestValidator : AbstractValidator<CreateProductRequest>
    {
        public CreateProductRequestValidator()
        {
            RuleFor(p => p.Name).MaximumLength(60).NotEmpty();
            RuleFor(p => p.StoreId).MaximumLength(255).NotEmpty();
            RuleFor(p => p.Description).MaximumLength(255).NotEmpty();
            RuleFor(p => p.CategoryId).GreaterThan(0);
            RuleFor(p => p.Handle).MaximumLength(60).NotEmpty();
        }
    }
}

public sealed record UpdateProductDetailsRequest(string Name, string Description)
{
    public class UpdateProductDetailsRequestValidator : AbstractValidator<UpdateProductDetailsRequest>
    {
        public UpdateProductDetailsRequestValidator()
        {
            RuleFor(p => p.Name).MaximumLength(60).NotEmpty();
            RuleFor(p => p.Description).MaximumLength(255).NotEmpty();
        }
    }
}

public sealed record UpdateProductPriceRequest(decimal Price);

public sealed record UpdateProductVatRateRequest(int? VatRateId);

public sealed record SetProductDiscountPriceRequest(decimal DiscountPrice);


public sealed record RestoreProductRegularPriceReguest();

public sealed record UpdateProductHandleRequest(string Handle)
{
    public class CreateHandleRequestValidator : AbstractValidator<UpdateProductHandleRequest>
    {
        public CreateHandleRequestValidator()
        {
            RuleFor(p => p.Handle).MaximumLength(60).NotEmpty();
        }
    }
}

public sealed record UpdateProductListingStateRequest(ProductListingState ListingState);

public sealed record UpdateProductCategoryRequest(int ProductCategoryId)
{
    public class UpdateProductCategoryRequestValidator : AbstractValidator<UpdateProductCategoryRequest>
    {
        public UpdateProductCategoryRequestValidator()
        {
            RuleFor(p => p.ProductCategoryId).GreaterThan(0);
        }
    }
}

public sealed record UpdateProductBrandRequest(long BrandId);

public sealed record ProductDto(
    int Id,
    string OrganizationId,
    string Name,
    StoreDto? Store,
    BrandDto? Brand,
    ProductCategory2? Category,
    ParentProductDto? Parent,
    string Description,
    decimal Price,
    double? VatRate,
    int? VatRateId,
    decimal? RegularPrice,
    double? DiscountRate,
    ProductImageDto? Image,
    IEnumerable<ProductImageDto> Images,
    string Handle,
    string? Sku,
    bool HasVariants,
    ProductListingState ListingState,
    IEnumerable<ProductAttributeDto> Attributes,
    IEnumerable<ProductOptionDto> Options
);

public record class ParentProductDto(
    int Id,
    string Name,
    ProductCategory? Category,
    string Description,
    decimal Price,
    decimal? RegularPrice,
    ProductImageDto? Image,
    string Handle);

public record class ProductAttributeDto(
    AttributeDto Attribute, AttributeValueDto? Value, bool ForVariant, bool IsMainAttribute);

public record class ProductOptionDto(
    OptionDto Option, bool IsInherited);

public sealed record UpdateProductSkuRequest(string Sku)
{
    public class UpdateProductSkuRequestValidator : AbstractValidator<UpdateProductSkuRequest>
    {
        public UpdateProductSkuRequestValidator()
        {
            RuleFor(p => p.Sku).MaximumLength(60).NotEmpty();
        }
    }
}

public record class ProductImageDto(string Id, string Title, string? Text, string Url);

public record class CreateProductImageData(string? Title, string? Text);

public record class UpdateProductImageData(string? Title, string? Text);

public record class SetMainProductImageData();

public record class CalculateProductPriceRequest(
        List<ProductOptionValue> OptionValues, string? SubscriptionPlanId = null);