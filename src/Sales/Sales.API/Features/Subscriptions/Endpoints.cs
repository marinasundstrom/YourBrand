using MediatR;

using YourBrand.Sales.Features.Subscriptions.Plans;
using YourBrand.Sales.Models;

namespace YourBrand.Sales.Features.Subscriptions;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapSubscriptionEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapSubscriptionPlanEndpoints();

        var versionedApi = app.NewVersionedApi("Subscriptions");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/subscriptions")
            .WithTags("Subscriptions")
            .HasApiVersion(ApiVersions.V1)
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("/", GetSubscriptions)
            .WithName($"Subscriptions_{nameof(GetSubscriptions)}");

        group.MapGet("/{id}", GetSubscriptionById)
            .WithName($"Subscriptions_{nameof(GetSubscriptionById)}");

        return app;
    }

    private static async Task<PagedResult<SubscriptionDto>> GetSubscriptions(IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetSubscriptionsQuery(), cancellationToken);
    }

    private static async Task<SubscriptionDto> GetSubscriptionById(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetSubscriptionQuery(id), cancellationToken);
    }
}