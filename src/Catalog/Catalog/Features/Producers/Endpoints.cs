using MediatR;

using YourBrand.Catalog.Features.Producers.Commands;
using YourBrand.Catalog.Features.Producers.Queries;
using YourBrand.Catalog.Model;

namespace YourBrand.Catalog.Features.Producers;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapProducersEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi("Producers");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/producers")
            .WithTags("Producers")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/", GetProducers)
            .WithName($"Producers_{nameof(GetProducers)}");

        group.MapGet("/{id}", GetProducerById)
            .WithName($"Producers_{nameof(GetProducerById)}");

        group.MapPost("/", CreateProducer)
            .WithName($"Producers_{nameof(CreateProducer)}");

        group.MapPut("/{id}", UpdateProducer)
            .WithName($"Producers_{nameof(UpdateProducer)}");

        group.MapDelete("/{id}", DeleteProducer)
            .WithName($"Producers_{nameof(DeleteProducer)}");

        return app;
    }

    public static async Task<PagedResult<ProducerDto>> GetProducers(string organizationId, string? productCategoryId = null, int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, SortDirection? sortDirection = null, IMediator mediator = default, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetProducersQuery(organizationId, productCategoryId, page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    public static async Task<ProducerDto?> GetProducerById(string organizationId, int id, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetProducerQuery(organizationId, id), cancellationToken);
    }

    public static async Task<ProducerDto> CreateProducer(string organizationId, CreateProducerDto dto, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateProducerCommand(organizationId, dto.Name, dto.Handle), cancellationToken);
    }

    public static async Task UpdateProducer(string organizationId, int id, UpdateProducerDto dto, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateProducerCommand(organizationId, id, dto.Name, dto.Handle), cancellationToken);
    }

    public static async Task DeleteProducer(string organizationId, int id, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteProducerCommand(organizationId, id), cancellationToken);
    }
}

public record CreateProducerDto(string Name, string Handle);

public record UpdateProducerDto(string Name, string Handle);