using MediatR;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Attributes;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapProductAttributesEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi("Products");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/products/{productId}/attributes")
            .WithTags("Products")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/", GetProductAttributes)
            .WithName($"Products_{nameof(GetProductAttributes)}");

        group.MapPost("/", AddProductAttribute)
            .WithName($"Products_{nameof(AddProductAttribute)}");

        group.MapPut("/{attributeId}", UpdateProductAttribute)
            .WithName($"Products_{nameof(UpdateProductAttribute)}");

        group.MapDelete("/{attributeId}", DeleteProductAttribute)
            .WithName($"Products_{nameof(DeleteProductAttribute)}");

        return app;
    }

    public static async Task<IEnumerable<ProductAttributeDto>> GetProductAttributes(string organizationId, long productId, IMediator mediator)
    {
        return await mediator.Send(new GetProductAttributes(organizationId, productId));
    }

    public static async Task<ProductAttributeDto> AddProductAttribute(string organizationId, long productId, AddProductAttributeDto data, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new AddProductAttribute(organizationId, productId, data.AttributeId, data.ValueId, data.ForVariant, data.IsMainAttribute), cancellationToken);
    }

    public static async Task<ProductAttributeDto> UpdateProductAttribute(string organizationId, long productId, string attributeId, UpdateProductAttributeDto data, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new UpdateProductAttribute(organizationId, productId, attributeId, data.ValueId, data.ForVariant, data.IsMainAttribute), cancellationToken);
    }

    public static async Task DeleteProductAttribute(string organizationId, long productId, string attributeId, IMediator mediator)
    {
        await mediator.Send(new DeleteProductAttribute(organizationId, productId, attributeId));
    }
}

public sealed record AddProductAttributeDto(string AttributeId, string ValueId, bool ForVariant, bool IsMainAttribute);

public sealed record UpdateProductAttributeDto(string ValueId, bool ForVariant, bool IsMainAttribute);