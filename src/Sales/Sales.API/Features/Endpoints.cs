using YourBrand.Sales.Features.OrderManagement;
using YourBrand.Sales.Features.Subscriptions;

namespace YourBrand.Sales.Features;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapFeaturesEndpoints(this IEndpointRouteBuilder app)
    {
        app
            .MapOrderManagementEndpoints()
            .MapSubscriptionEndpoints();

        return app;
    }
}