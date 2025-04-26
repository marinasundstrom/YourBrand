using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Catalog.Features.ProductManagement.Options;
using YourBrand.Catalog.Features.ProductManagement.Products.Options.Groups;
using YourBrand.Catalog.Model;

namespace YourBrand.Catalog.Features.ProductManagement.SubscriptionPlans.SubscriptionPlans;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapProductSubscriptionPlansEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi("ProductSubscriptionPlans");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/products/{idOrId}/subscriptionPlans")
            .WithTags("Products")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi()
            .RequireAuthorization();


        group.MapGet("/", GetProductSubscriptionPlans)
            .WithName($"Products_{nameof(GetProductSubscriptionPlans)}");

        return app;
    }

    private static async Task<Ok<PagedResult<ProductSubscriptionPlanDto>>> GetProductSubscriptionPlans(string organizationId, string? storeId = null, string? idOrId = null,
        int page = 1, int pageSize = 10, string? sortBy = null, SortDirection? sortDirection = null, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var pagedResult = await mediator.Send(new GetProductSubscriptionPlans(organizationId, storeId, idOrId, page, pageSize, sortBy, sortDirection), cancellationToken);
        return TypedResults.Ok(pagedResult);
    }
}