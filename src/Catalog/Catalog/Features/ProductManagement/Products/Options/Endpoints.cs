using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Catalog.Features.ProductManagement.Options;
using YourBrand.Catalog.Features.ProductManagement.Products.Options.Groups;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Options;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapProductOptionsEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi("Products");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/products/{id}/options")
            .WithTags("Products")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi()
            .RequireAuthorization();


        group.MapGet("/", GetProductOptions)
            .WithName($"ProductOptions_{nameof(GetProductOptions)}");

        // group.MapGet("/{id}", GetProductOptionById)
        //    .WithName($"ProductOptions_{nameof(GetProductOptionById)}");

        group.MapPost("/", CreateProductOption)
            .WithName($"ProductOptions_{nameof(CreateProductOption)}");

        group.MapPut("/{optionId}", UpdateProductOption)
            .WithName($"ProductOptions_{nameof(UpdateProductOption)}");

        group.MapDelete("/{optionId}", DeleteProductOption)
            .WithName($"ProductOptions_{nameof(DeleteProductOption)}");

        group.MapPost("/{optionId}/values", CreateProductOptionValue)
            .WithName($"ProductOptions_{nameof(CreateProductOptionValue)}");

        group.MapDelete("/{optionId}/values/{valueId}", DeleteProductOptionValue)
            .WithName($"ProductOptions_{nameof(DeleteProductOptionValue)}");

        group.MapGet("/groups", GetProductOptionGroups)
            .WithName($"ProductOptions_{nameof(GetProductOptionGroups)}");

        //group.MapGet("/groups/{optionGroupId}", GetProductOptionGroupById)
        //    .WithName($"ProductOptions_{nameof(GetProductOptionGroupById)}");

        group.MapPost("/groups", CreateProductOptionGroup)
            .WithName($"ProductOptions_{nameof(CreateProductOptionGroup)}");

        group.MapPut("/groups/{optionGroupId}", UpdateProductOptionGroup)
            .WithName($"ProductOptions_{nameof(UpdateProductOptionGroup)}");

        group.MapDelete("/groups/{optionGroupId}", DeleteProductOptionGroup)
            .WithName($"ProductOptions_{nameof(DeleteProductOptionGroup)}");

        return app;
    }

    public static async Task<Results<Ok<IEnumerable<ProductOptionDto>>, BadRequest>> GetProductOptions(string organizationId, long id, string? variantId, IMediator mediator, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await mediator.Send(new GetProductOptions(organizationId, id, variantId), cancellationToken));
    }

    public static async Task<Results<Ok<OptionDto>, BadRequest>> CreateProductOption(string organizationId, long id, CreateProductOptionData data, IMediator mediator, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await mediator.Send(new CreateProductOption(organizationId, id, data), cancellationToken));
    }

    public static async Task<Results<Ok<OptionDto>, BadRequest>> UpdateProductOption(string organizationId, long id, string optionId, UpdateProductOptionData data, IMediator mediator, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await mediator.Send(new UpdateProductOption(organizationId, id, optionId, data), cancellationToken));
    }

    public static async Task<Results<Ok, BadRequest>> DeleteProductOption(string organizationId, long id, string optionId, IMediator mediator)
    {
        await mediator.Send(new DeleteProductOption(organizationId, id, optionId));
        return TypedResults.Ok();
    }

    public static async Task<Results<Ok<OptionValueDto>, BadRequest>> CreateProductOptionValue(string organizationId, long id, string optionId, CreateProductOptionValueData data, IMediator mediator, CancellationToken cancellationToken)
    {

        return TypedResults.Ok(await mediator.Send(new CreateProductOptionValue(organizationId, id, optionId, data), cancellationToken));
    }

    public static async Task<Results<Ok, BadRequest>> DeleteProductOptionValue(string organizationId, long id, string optionId, string valueId, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteProductOptionValue(organizationId, id, optionId, valueId), cancellationToken);
        return TypedResults.Ok();
    }

    public static async Task<Results<Ok<IEnumerable<OptionValueDto>>, BadRequest>> GetProductOptionValues(string organizationId, long id, string optionId, IMediator mediator)
    {
        return TypedResults.Ok(await mediator.Send(new GetOptionValues(organizationId, optionId)));
    }

    public static async Task<Results<Ok<IEnumerable<OptionGroupDto>>, BadRequest>> GetProductOptionGroups(string organizationId, long id, IMediator mediator)
    {
        return TypedResults.Ok(await mediator.Send(new GetProductOptionGroups(organizationId, id)));
    }

    public static async Task<Results<Ok<OptionGroupDto>, BadRequest>> CreateProductOptionGroup(string organizationId, long id, CreateProductOptionGroupData data, IMediator mediator)
    {
        return TypedResults.Ok(await mediator.Send(new CreateProductOptionGroup(organizationId, id, data)));
    }

    public static async Task<Results<Ok<OptionGroupDto>, BadRequest>> UpdateProductOptionGroup(string organizationId, long id, string optionGroupId, UpdateProductOptionGroupData data, IMediator mediator)
    {
        return TypedResults.Ok(await mediator.Send(new UpdateProductOptionGroup(organizationId, id, optionGroupId, data)));
    }

    public static async Task<Results<Ok, BadRequest>> DeleteProductOptionGroup(string organizationId, long id, string optionGroupId, IMediator mediator)
    {
        await mediator.Send(new DeleteProductOptionGroup(organizationId, id, optionGroupId));
        return TypedResults.Ok();
    }
}