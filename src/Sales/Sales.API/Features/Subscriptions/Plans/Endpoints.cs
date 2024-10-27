using MediatR;

using YourBrand.Sales.Models;

namespace YourBrand.Sales.Features.SubscriptionManagement.Plans;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapSubscriptionPlanEndpoints(this IEndpointRouteBuilder app)
    {
        var versionedApi = app.NewVersionedApi("SubscriptionPlans");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/subscriptionPlans")
            .WithTags("SubscriptionPlans")
            .HasApiVersion(ApiVersions.V1)
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("/", GetSubscriptionPlans)
            .WithName($"SubscriptionPlans_{nameof(GetSubscriptionPlans)}");

        group.MapGet("/{id}", GetSubscriptionPlanById)
            .WithName($"SubscriptionPlans_{nameof(GetSubscriptionPlanById)}");

        return app;
    }

    private static async Task<PagedResult<SubscriptionPlanDto>> GetSubscriptionPlans(IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetSubscriptionPlansQuery(), cancellationToken);
    }

    private static async Task<SubscriptionPlanDto> GetSubscriptionPlanById(Guid id, IMediator mediator, CancellationToken cancellationToken)
    {
        return await mediator.Send(new GetSubscriptionPlanQuery(id), cancellationToken);
    }
}