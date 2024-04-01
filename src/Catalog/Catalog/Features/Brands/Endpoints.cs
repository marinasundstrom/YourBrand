using MediatR;

using YourBrand.Catalog.Features.Brands.Commands;
using YourBrand.Catalog.Features.Brands.Queries;
using YourBrand.Catalog.Model;

namespace YourBrand.Catalog.Features.Brands;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapBrandsEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi("Brands");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/brands")
            .WithTags("Brands")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi()
            .RequireAuthorization();

        group.MapGet("/", GetBrands)
            .WithName($"Brands_{nameof(GetBrands)}");

        group.MapGet("/{id}", GetBrandById)
            .WithName($"Brands_{nameof(GetBrandById)}");

        group.MapPost("/", CreateBrand)
            .WithName($"Brands_{nameof(CreateBrand)}");

        group.MapPut("/{id}", UpdateBrand)
            .WithName($"Brands_{nameof(UpdateBrand)}");

        group.MapDelete("/{id}", DeleteBrand)
            .WithName($"Brands_{nameof(DeleteBrand)}");

        return app;
    }

    public static async Task<PagedResult<BrandDto>> GetBrands(string? productCategoryId = null, int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, SortDirection? sortDirection = null, IMediator mediator = default, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetBrandsQuery(productCategoryId, page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    public static async Task<BrandDto?> GetBrandById(int id, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetBrandQuery(id), cancellationToken);
    }

    public static async Task<BrandDto> CreateBrand(CreateBrandDto dto, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateBrandCommand(dto.Name, dto.Handle), cancellationToken);
    }

    public static async Task UpdateBrand(int id, UpdateBrandDto dto, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateBrandCommand(id, dto.Name, dto.Handle), cancellationToken);
    }

    public static async Task DeleteBrand(int id, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteBrandCommand(id), cancellationToken);
    }
}

public record CreateBrandDto(string Name, string Handle);

public record UpdateBrandDto(string Name, string Handle);