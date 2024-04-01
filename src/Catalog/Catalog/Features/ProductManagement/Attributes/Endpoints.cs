using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

using YourBrand.Catalog.Features.ProductManagement.Attributes.Groups;
using YourBrand.Catalog.Features.ProductManagement.Attributes.Values;
using YourBrand.Catalog.Model;

namespace YourBrand.Catalog.Features.ProductManagement.Attributes;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapAttributesEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi("Attributes");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/attributes")
            .WithTags("Attributes")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi();

        group.MapGet("/", GetAttributes)
            .WithName($"Attributes_{nameof(GetAttributes)}");

        group.MapGet("/{id}", GetAttributeById)
            .WithName($"Attributes_{nameof(GetAttributeById)}");

        group.MapPost("/", CreateAttribute)
            .WithName($"Attributes_{nameof(CreateAttribute)}");

        group.MapPut("/{id}", UpdateAttribute)
            .WithName($"Attributes_{nameof(UpdateAttribute)}");

        group.MapDelete("/{id}", DeleteAttribute)
            .WithName($"Attributes_{nameof(DeleteAttribute)}");


        group.MapPost("/values", CreateAttributeValue)
            .WithName($"Attributes_{nameof(CreateAttributeValue)}");

        group.MapDelete("/values/{valueId}", DeleteAttributeValue)
            .WithName($"Attributes_{nameof(DeleteAttributeValue)}");


        group.MapGet("/groups", GetAttributeGroups)
            .WithName($"Attributes_{nameof(GetAttributeGroups)}");

        /*
                group.MapGet("/groups/{id}", GetAttributeById)
                    .WithName($"Attributes_{nameof(GetAttributeById)}");
                    */

        group.MapPost("/groups", CreateAttributeGroup)
            .WithName($"Attributes_{nameof(CreateAttributeGroup)}");

        group.MapPut("/groups/{id}", UpdateAttributeGroup)
            .WithName($"Attributes_{nameof(UpdateAttributeGroup)}");

        group.MapDelete("/groups/{id}", DeleteAttributeGroup)
            .WithName($"Attributes_{nameof(DeleteAttributeGroup)}");

        return app;
    }

    public static async Task<Results<Ok<PagedResult<AttributeDto>>, BadRequest>> GetAttributes(
         [FromQuery] string[]? ids = null, int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, SortDirection? sortDirection = null, IMediator mediator = default, CancellationToken cancellationToken = default)
    {
        return TypedResults.Ok(await mediator.Send(new GetAttributes(ids, page, pageSize, searchString, sortBy, sortDirection), cancellationToken));
    }

    public static async Task<AttributeDto> GetAttributeById(string id, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetAttribute(id));
    }

    public static async Task<Results<Ok<IEnumerable<AttributeValueDto>>, BadRequest>> GetAttributesValues(string id, IMediator mediator, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await mediator.Send(new GetAttributeValues(id), cancellationToken));
    }

    public static async Task<Results<Ok<AttributeDto?>, BadRequest>> CreateAttribute(CreateAttributeDto dto, IMediator mediator, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await mediator.Send(new CreateAttributeCommand(dto.Name, dto.Description, dto.GroupId, dto.Values), cancellationToken));
    }

    public static async Task UpdateAttribute(string id, UpdateAttributeDto dto, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateAttributeCommand(id, dto.Name, dto.Description, dto.GroupId, dto.Values), cancellationToken);
    }

    public static async Task DeleteAttribute(string id, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteAttributeCommand(id), cancellationToken);
    }

    public static async Task<Results<Ok<AttributeValueDto>, BadRequest>> CreateAttributeValue(string id, CreateProductAttributeValueData data, IMediator mediator, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await mediator.Send(new CreateProductAttributeValue(id, data), cancellationToken));
    }

    public static async Task<Results<Ok, BadRequest>> DeleteAttributeValue(string id, string valueId, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteProductAttributeValue(id, valueId), cancellationToken);
        return TypedResults.Ok();
    }

    public static async Task<Results<Ok<IEnumerable<AttributeGroupDto>>, BadRequest>> GetAttributeGroups(IMediator mediator)
    {
        return TypedResults.Ok(await mediator.Send(new GetAttributeGroups()));
    }

    public static async Task<Results<Ok<AttributeGroupDto>, BadRequest>> CreateAttributeGroup(CreateProductAttributeGroupData data, IMediator mediator)
    {
        return TypedResults.Ok(await mediator.Send(new CreateAttributeGroup(data)));
    }

    public static async Task<Results<Ok<AttributeGroupDto>, BadRequest>> UpdateAttributeGroup(string id, UpdateProductAttributeGroupData data, IMediator mediator)
    {
        return TypedResults.Ok(await mediator.Send(new UpdateAttributeGroup(id, data)));
    }

    public static async Task<Results<Ok, BadRequest>> DeleteAttributeGroup(string id, IMediator mediator)
    {
        await mediator.Send(new DeleteAttributeGroup(id));
        return TypedResults.Ok();
    }
}

public record CreateAttributeDto(string Name, string? Description, string? GroupId, IEnumerable<CreateProductAttributeValueData> Values);

public record UpdateAttributeDto(string Name, string? Description, string? GroupId, IEnumerable<UpdateProductAttributeValueData> Values);