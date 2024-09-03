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

    public static async Task<PagedResult<BrandDto>> GetBrands(string organizationId, string? productCategoryId = null, int page = 1, int pageSize = 10, string? searchString = null, string? sortBy = null, SortDirection? sortDirection = null, IMediator mediator = default, CancellationToken cancellationToken = default)
    {
        return await mediator.Send(new GetBrandsQuery(organizationId, productCategoryId, page, pageSize, searchString, sortBy, sortDirection), cancellationToken);
    }

    public static async Task<BrandDto?> GetBrandById(string organizationId, int id, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetBrandQuery(organizationId, id), cancellationToken);
    }

    public static async Task<BrandDto> CreateBrand(string organizationId, CreateBrandDto dto, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new CreateBrandCommand(organizationId, dto.Name, dto.Handle), cancellationToken);
    }

    public static async Task UpdateBrand(string organizationId, int id, UpdateBrandDto dto, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateBrandCommand(organizationId, id, dto.Name, dto.Handle), cancellationToken);
    }

    public static async Task DeleteBrand(string organizationId, int id, IMediator mediator, CancellationToken cancellationToken)
    {
        await mediator.Send(new DeleteBrandCommand(organizationId, id), cancellationToken);
    }
}

public record CreateBrandDto(string Name, string Handle);

public record UpdateBrandDto(string Name, string Handle);