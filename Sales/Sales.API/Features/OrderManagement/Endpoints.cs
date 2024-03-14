using YourBrand.Sales.API.Features.OrderManagement.Orders;
using YourBrand.Sales.API.Features.OrderManagement.Orders.Statuses;

namespace YourBrand.Sales.API.Features.OrderManagement;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapOrderManagementEndpoints(this IEndpointRouteBuilder app)
    {
        app
            .MapOrderEndpoints()
            .MapOrderStatusEndpoints();

        return app;
    }
}