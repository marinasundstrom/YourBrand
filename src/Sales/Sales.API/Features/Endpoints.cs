using YourBrand.Sales.Features.OrderManagement;
using YourBrand.Sales.Features.SubscriptionManagement;
using YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Statuses;
using YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Types;

namespace YourBrand.Sales.Features;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapFeaturesEndpoints(this IEndpointRouteBuilder app)
    {
        app
            .MapOrderManagementEndpoints()
            .MapSubscriptionEndpoints()
            .MapSubscriptionTypeEndpoints()
            .MapSubscriptionStatusEndpoints();

        return app;
    }
}