using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.OrderManagement.Orders.Statuses;
using YourBrand.Sales.Features.OrderManagement.Orders.Types;

namespace YourBrand.Sales.Features.OrderManagement;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapOrderManagementEndpoints(this IEndpointRouteBuilder app)
    {
        app
            .MapOrderEndpoints()
            .MapOrderTypeEndpoints()
            .MapOrderStatusEndpoints();

        return app;
    }
}