using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Customers.Client;
using YourBrand.Sales;
using YourBrand.StoreFront.API;

namespace YourBrand.StoreFront.Application.Features.UserProfiles;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapOrdersEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi("Orders");

        var ordersGroup = versionedApi.MapGroup("/v{version:apiVersion}/orders")
            .WithTags("Orders")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi()
            .RequireCors();

        ordersGroup.MapGet("/", GetOrders)
            .WithName($"Orders_{nameof(GetOrders)}");
            //.RequireAuthorization()
            //.CacheOutput(OutputCachePolicyNames.GetOrders);

        return app;
    }

    private static async Task<Results<Ok<PagedResult<Order>>, NotFound>> GetOrders(int? page = 1, int? pageSize = 10, string? searchTerm = null, IUserContext? userContext = null, ICustomersClient? customersClient = null, IConfiguration? configuration = null, IOrdersClient ordersClient = default!, CancellationToken cancellationToken = default)
    {
        string? ssn = userContext.GetClaim("ssn");

        var customer = await customersClient.GetCustomerBySSNAsync(ssn, cancellationToken);

        var orders = await ordersClient.GetOrdersAsync(configuration["OrganizationId"]!, null, null, null, customer.Ssn, null, null, page, pageSize, null, null, cancellationToken);

        return TypedResults.Ok(new PagedResult<Order>(orders.Items, orders.Total));
    }
}