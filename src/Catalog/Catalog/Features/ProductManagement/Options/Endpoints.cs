using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

namespace YourBrand.Catalog.Features.ProductManagement.Options;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapOptionsEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi("Options");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/options")
            .WithTags("Options")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi();

        group.MapGet("/", GetOptionValues)
            .WithName($"Options_{nameof(GetOptions)}")
            .Produces<IEnumerable<OptionDto>>();

        group.MapGet("/{id}/values", GetOptionValues)
            .WithName($"Options_{nameof(GetOptionValues)}")
            .Produces<IEnumerable<OptionValueDto>>();

        return app;
    }

    public static async Task<Results<Ok<IEnumerable<OptionDto>>, BadRequest>> GetOptions(string organizationId, bool includeChoices = false, IMediator mediator = default)
    {
        return TypedResults.Ok(await mediator.Send(new GetOptions(organizationId, includeChoices)));
    }

    /*
    [HttpGet("{optionId}")]
    public async Task<IResult<OptionDto>> GetProductOptionValues(string organizationId, string optionId, IMediator mediator = default)
    {
        return TypedResults.Ok(await _mediator.Send(new GetOption(optionId)));
    }
    */

    public static async Task<Results<Ok<IEnumerable<OptionValueDto>>, BadRequest>> GetOptionValues(string organizationId, string id, IMediator mediator = default)
    {
        return TypedResults.Ok(await mediator.Send(new GetOptionValues(organizationId, id)));
    }
}