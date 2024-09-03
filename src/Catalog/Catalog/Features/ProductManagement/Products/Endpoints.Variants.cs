using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Catalog.Features.ProductManagement.Attributes;
using YourBrand.Catalog.Features.ProductManagement.Products.Variants;
using YourBrand.Catalog.Model;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public static partial class Endpoints
{
    public static IEndpointRouteBuilder MapProductVariantsEndpoints(this IEndpointRouteBuilder app)
    {
        string GetProductVariantsExpire20 = nameof(GetProductVariantsExpire20);

        var versionedApi = app.NewVersionedApi("Products");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/products")
            .WithTags("Products")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("{idOrHandle}/variants", GetVariants)
            .WithName($"Products_{nameof(GetVariants)}")
            .CacheOutput(GetProductVariantsExpire20);

        group.MapGet("{idOrHandle}/variants/{variantIdOrHandle}", GetVariant)
            .WithName($"Products_{nameof(GetVariant)}");

        group.MapDelete("{id}/variants{variantId}", DeleteVariant)
            .WithName($"Products_{nameof(DeleteVariant)}");

        group.MapPost("{idOrHandle}/variants/findVariant", FindVariantByAttributeValues)
            .WithName($"Products_{nameof(FindVariantByAttributeValues)}");

        group.MapPost("{idOrHandle}/variants/find2", FindsVariantsByAttributeValues)
            .WithName($"Products_{nameof(FindsVariantsByAttributeValues)}");

        group.MapPost("{idOrHandle}/attributes/{attributeId}/availableValuesForVariant", GetAvailableVariantAttributeValues)
            .WithName($"Products_{nameof(GetAvailableVariantAttributeValues)}");

        group.MapPost("{id}/variants", CreateVariant)
            .WithName($"Products_{nameof(CreateVariant)}")
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPut("{id}/variants/{variantId}", UpdateVariant)
            .WithName($"Products_{nameof(UpdateVariant)}")
            .ProducesProblem(StatusCodes.Status400BadRequest);

        group.MapPost("{id}/variants/{variantId}/uploadImage", UploadVariantImage)
            .WithName($"Products_{nameof(UploadVariantImage)}");

        return app;
    }

    public static async Task<Results<Ok<PagedResult<ProductDto>>, BadRequest>> GetVariants(string organizationId, string idOrHandle, int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, SortDirection? sortDirection = null, IMediator mediator = default, CancellationToken cancellationToken = default)
    {
        return TypedResults.Ok(await mediator.Send(new GetProductVariants(organizationId, idOrHandle, page, pageSize, searchString, sortBy, sortDirection)));
    }

    public static async Task<Results<Ok, BadRequest>> DeleteVariant(string organizationId, long id, long variantId, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteProductVariant(organizationId, id, variantId), cancellationToken);
        return TypedResults.Ok();
    }

    public static async Task<Results<Ok<ProductDto>, BadRequest>> GetVariant(string organizationId, string idOrHandle, string variantIdOrHandle, IMediator mediator, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await mediator.Send(new GetProductVariant(organizationId, idOrHandle, variantIdOrHandle), cancellationToken));
    }

    public static async Task<Results<Ok<ProductDto?>, BadRequest>> FindVariantByAttributeValues(string organizationId, string idOrHandle, Dictionary<string, string?> selectedAttributeValues, IMediator mediator, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await mediator.Send(new FindProductVariant(organizationId, idOrHandle, selectedAttributeValues), cancellationToken));
    }

    public static async Task<Results<Ok<IEnumerable<ProductDto>>, BadRequest>> FindsVariantsByAttributeValues(string organizationId, string idOrHandle, Dictionary<string, string?> selectedAttributeValues, IMediator mediator, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await mediator.Send(new FindProductVariants(organizationId, idOrHandle, selectedAttributeValues), cancellationToken));
    }

    public static async Task<Results<Ok<IEnumerable<AttributeValueDto>>, BadRequest>> GetAvailableVariantAttributeValues(string organizationId, string idOrHandle, string attributeId, Dictionary<string, string?> selectedAttributeValues, IMediator mediator, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await mediator.Send(new GetAvailableAttributeValues(organizationId, idOrHandle, attributeId, selectedAttributeValues), cancellationToken));
    }

    public static async Task<Results<Ok<ProductDto>, ProblemHttpResult>> CreateVariant(string organizationId, long id, CreateProductVariantData data, IMediator mediator, CancellationToken cancellationToken)
    {
        try
        {
            return TypedResults.Ok(await mediator.Send(new CreateProductVariant(organizationId, id, data), cancellationToken));
        }
        catch (VariantAlreadyExistsException e)
        {
            return TypedResults.Problem(
                title: "Variant already exists.",
                detail: "There is already a variant with the chosen options.",
                //instance: Request.Path,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }

    public static async Task<Results<Ok<ProductDto>, ProblemHttpResult>> UpdateVariant(string organizationId, long id, long variantId, UpdateProductVariantData data, IMediator mediator, CancellationToken cancellationToken)
    {
        try
        {
            return TypedResults.Ok(await mediator.Send(new UpdateProductVariant(organizationId, id, variantId, data), cancellationToken));
        }
        catch (VariantAlreadyExistsException e)
        {
            return TypedResults.Problem(
                title: "Variant already exists.",
                detail: "There is already a variant with the chosen options.",
                //instance: Request.Path,
                statusCode: StatusCodes.Status400BadRequest);
        }
    }

    public static async Task<Results<Ok<string>, BadRequest>> UploadVariantImage(string organizationId, long id, long variantId, IFormFile file, IMediator mediator, CancellationToken cancellationToken)
    {
        var url = await mediator.Send(new UploadProductVariantImage(organizationId, id, variantId, file.Name, file.OpenReadStream()), cancellationToken);
        return TypedResults.Ok(url);
    }
}