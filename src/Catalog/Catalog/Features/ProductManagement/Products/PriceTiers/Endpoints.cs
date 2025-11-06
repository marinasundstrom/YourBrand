using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Features.ProductManagement.Products;
using YourBrand.Extensions;

namespace YourBrand.Catalog.Features.ProductManagement.Products.PriceTiers;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapProductPriceTiersEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi("Products");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/products/{idOrHandle}/price/tiers")
            .WithTags("Products")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/", GetProductPriceTiers)
            .WithName($"Products_{nameof(GetProductPriceTiers)}");

        group.MapPost("/", CreateProductPriceTier)
            .AddEndpointFilter<ValidationFilter<CreateProductPriceTierRequest>>()
            .WithName($"Products_{nameof(CreateProductPriceTier)}");

        group.MapPut("/{tierId}", UpdateProductPriceTier)
            .AddEndpointFilter<ValidationFilter<UpdateProductPriceTierRequest>>()
            .WithName($"Products_{nameof(UpdateProductPriceTier)}");

        group.MapDelete("/{tierId}", DeleteProductPriceTier)
            .WithName($"Products_{nameof(DeleteProductPriceTier)}");

        return app;
    }

    private static async Task<Results<Ok<IEnumerable<ProductPriceTierDto>>, NotFound>> GetProductPriceTiers(
        string organizationId,
        string idOrHandle,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetProductPriceTiers(organizationId, idOrHandle), cancellationToken);

        return result.IsSuccess
            ? TypedResults.Ok(result.GetValue())
            : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<ProductPriceTierDto>, NotFound, BadRequest>> CreateProductPriceTier(
        string organizationId,
        string idOrHandle,
        CreateProductPriceTierRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateProductPriceTier(
            organizationId,
            idOrHandle,
            request.FromQuantity,
            request.ToQuantity,
            request.TierType,
            request.Value),
            cancellationToken);

        if (result.IsFailure)
        {
            return result.HasError(Products.Errors.ProductNotFound)
                ? TypedResults.NotFound()
                : TypedResults.BadRequest();
        }

        return TypedResults.Ok(result.GetValue());
    }

    private static async Task<Results<Ok<ProductPriceTierDto>, NotFound, BadRequest>> UpdateProductPriceTier(
        string organizationId,
        string idOrHandle,
        string tierId,
        UpdateProductPriceTierRequest request,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new UpdateProductPriceTier(
            organizationId,
            idOrHandle,
            tierId,
            request.FromQuantity,
            request.ToQuantity,
            request.TierType,
            request.Value),
            cancellationToken);

        if (result.IsFailure)
        {
            if (result.HasError(Products.Errors.ProductNotFound) || result.HasError(Products.Errors.ProductPriceTierNotFound))
            {
                return TypedResults.NotFound();
            }

            return TypedResults.BadRequest();
        }

        return TypedResults.Ok(result.GetValue());
    }

    private static async Task<Results<NoContent, NotFound, BadRequest>> DeleteProductPriceTier(
        string organizationId,
        string idOrHandle,
        string tierId,
        IMediator mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new DeleteProductPriceTier(organizationId, idOrHandle, tierId), cancellationToken);

        if (result.IsFailure)
        {
            if (result.HasError(Products.Errors.ProductNotFound) || result.HasError(Products.Errors.ProductPriceTierNotFound))
            {
                return TypedResults.NotFound();
            }

            return TypedResults.BadRequest();
        }

        return TypedResults.NoContent();
    }
}

public sealed record CreateProductPriceTierRequest(
    int FromQuantity,
    int? ToQuantity,
    ProductPriceTierType TierType,
    decimal Value);

public sealed record UpdateProductPriceTierRequest(
    int FromQuantity,
    int? ToQuantity,
    ProductPriceTierType TierType,
    decimal Value);

public sealed class CreateProductPriceTierRequestValidator : AbstractValidator<CreateProductPriceTierRequest>
{
    public CreateProductPriceTierRequestValidator()
    {
        RuleFor(x => x.FromQuantity).GreaterThan(0);
        RuleFor(x => x.ToQuantity).GreaterThanOrEqualTo(x => x.FromQuantity).When(x => x.ToQuantity.HasValue);
        RuleFor(x => x.Value).GreaterThanOrEqualTo(0);
    }
}

public sealed class UpdateProductPriceTierRequestValidator : AbstractValidator<UpdateProductPriceTierRequest>
{
    public UpdateProductPriceTierRequestValidator()
    {
        RuleFor(x => x.FromQuantity).GreaterThan(0);
        RuleFor(x => x.ToQuantity).GreaterThanOrEqualTo(x => x.FromQuantity).When(x => x.ToQuantity.HasValue);
        RuleFor(x => x.Value).GreaterThanOrEqualTo(0);
    }
}
