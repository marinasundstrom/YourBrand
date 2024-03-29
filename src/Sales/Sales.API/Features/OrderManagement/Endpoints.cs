using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.OrderManagement.Orders.Statuses;

namespace YourBrand.Sales.Features.OrderManagement;

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