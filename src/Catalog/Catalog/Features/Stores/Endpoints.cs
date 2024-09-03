using MediatR;

using YourBrand.Catalog.Features.Stores.Commands;
using YourBrand.Catalog.Features.Stores.Queries;
using YourBrand.Catalog.Model;

namespace YourBrand.Catalog.Features.Stores;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapStoresEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi("Stores");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/stores")
            .WithTags("Stores")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/", GetStores)
            .WithName($"Stores_{nameof(GetStores)}");

        group.MapGet("/{IdOrHandle}", GetStoreById)
            .WithName($"Stores_{nameof(GetStoreById)}");

        group.MapPost("/", CreateStore)
            .WithName($"Stores_{nameof(CreateStore)}");

        group.MapPut("/{id}", UpdateStore)
            .WithName($"Stores_{nameof(UpdateStore)}");

        group.MapDelete("/{id}", DeleteStore)
            .WithName($"Stores_{nameof(DeleteStore)}");

        return app;
    }

    public static async Task<PagedResult<StoreDto>> GetStores(string organizationId, int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, SortDirection? sortDirection = null, IMediator mediator = default, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetStoresQuery(organizationId, page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    public static async Task<StoreDto?> GetStoreById(string organizationId, string idOrHandle, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetStoreQuery(organizationId, idOrHandle), cancellationToken);
    }

    public static async Task<StoreDto> CreateStore(string organizationId, CreateStoreDto dto, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateStoreCommand(organizationId, dto.Name, dto.Handle, dto.Currency), cancellationToken);
    }

    public static async Task UpdateStore(string organizationId, string id, UpdateStoreDto dto, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateStoreCommand(organizationId, id, dto.Name, dto.Handle, dto.Currency), cancellationToken);
    }

    public static async Task DeleteStore(string organizationId, string id, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteStoreCommand(organizationId, id), cancellationToken);
    }
}

public record CreateStoreDto(string Name, string Handle, string Currency);

public record UpdateStoreDto(string Name, string Handle, string Currency);