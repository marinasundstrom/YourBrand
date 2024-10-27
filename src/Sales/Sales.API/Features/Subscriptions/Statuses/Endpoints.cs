using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Statuses.Commands;
using YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Statuses.Queries;
using YourBrand.Sales.Models;

namespace YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Statuses;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapSubscriptionStatusEndpoints(this IEndpointRouteBuilder app)
    {
        string GetSubscriptionsExpire20 = nameof(GetSubscriptionsExpire20);

        var versionedApi = app.NewVersionedApi("SubscriptionStatuses");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/subscriptions/statuses")
            .WithTags("SubscriptionStatuses")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi();

        group.MapGet("/", GetSubscriptionStatuses)
            .WithName($"SubscriptionStatuses_{nameof(GetSubscriptionStatuses)}");

        group.MapGet("/{id}", GetSubscriptionStatusById)
            .WithName($"SubscriptionStatuses_{nameof(GetSubscriptionStatusById)}");

        group.MapPost("/", CreateSubscriptionStatus)
            .WithName($"SubscriptionStatuses_{nameof(CreateSubscriptionStatus)}");

        group.MapPost("{id}", UpdateSubscriptionStatus)
            .WithName($"SubscriptionStatuses_{nameof(UpdateSubscriptionStatus)}");

        group.MapDelete("{id}", DeleteSubscriptionStatus)
            .WithName($"SubscriptionStatuses_{nameof(DeleteSubscriptionStatus)}");

        return app;
    }

    private static async Task<Ok<PagedResult<SubscriptionStatusDto>>> GetSubscriptionStatuses(string organizationId, int page = 1, int pageSize = 10, string? searchTerm = null, string? sortBy = null, SortDirection? sortDirection = null, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new GetSubscriptionStatusesQuery(organizationId, page, pageSize, searchTerm, sortBy, sortDirection), cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<SubscriptionStatusDto>, NotFound>> GetSubscriptionStatusById(string organizationId, int id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSubscriptionStatusQuery(organizationId, id), cancellationToken);

        /*
        if (result.HasError(Errors.Subscriptions.SubscriptionNotFound))
        {
            return TypedResults.NotFound();
        }
        */

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Created<SubscriptionStatusDto>, NotFound>> CreateSubscriptionStatus(string organizationId, CreateSubscriptionStatusDto request, IMediator mediator, LinkGenerator linkGenerator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateSubscriptionStatusCommand(organizationId, request.Name, request.Handle, request.Description), cancellationToken);

        /*
        if (result.HasError(Errors.Subscriptions.SubscriptionNotFound))
        {
            return TypedResults.NotFound();
        }

        var subscription = result.GetValue();*/

        var path = linkGenerator.GetPathByName(nameof(GetSubscriptionStatusById), new { id = result.Id });

        return TypedResults.Created(path, result);
    }

    private static async Task<Results<Ok, NotFound>> UpdateSubscriptionStatus(string organizationId, int id, UpdateSubscriptionStatusDto request, IMediator mediator, LinkGenerator linkGenerator, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateSubscriptionStatusCommand(organizationId, id, request.Name, request.Handle, request.Description), cancellationToken);

        /*
        if (result.HasError(Errors.Subscriptions.SubscriptionNotFound))
        {
            return TypedResults.NotFound();
        }

        var subscription = result.GetValue();*/

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> DeleteSubscriptionStatus(string organizationId, int id, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        await mediator.Send(new DeleteSubscriptionStatusCommand(organizationId, id), cancellationToken);

        /*
        if (result.HasError(Errors.Subscriptions.SubscriptionNotFound))
        {
            return TypedResults.NotFound();
        }

        if (result.HasError(Errors.Subscriptions.SubscriptionItemNotFound))
        {
            return TypedResults.NotFound();
        }
        */

        return TypedResults.Ok();
    }
}

public record CreateSubscriptionStatusDto(string Name, string Handle, string? Description);

public record UpdateSubscriptionStatusDto(string Name, string Handle, string? Description);