using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Types.Commands;
using YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Types.Queries;
using YourBrand.Sales.Models;

namespace YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Types;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapSubscriptionTypeEndpoints(this IEndpointRouteBuilder app)
    {
        string GetSubscriptionsExpire20 = nameof(GetSubscriptionsExpire20);

        var versionedApi = app.NewVersionedApi("SubscriptionTypes");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/subscriptions/types")
            .WithTags("SubscriptionTypes")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi();

        group.MapGet("/", GetSubscriptionTypes)
            .WithName($"SubscriptionTypes_{nameof(GetSubscriptionTypes)}");

        group.MapGet("/{id}", GetSubscriptionTypeById)
            .WithName($"SubscriptionTypes_{nameof(GetSubscriptionTypeById)}");

        group.MapPost("/", CreateSubscriptionType)
            .WithName($"SubscriptionTypes_{nameof(CreateSubscriptionType)}");

        group.MapPost("{id}", UpdateSubscriptionType)
            .WithName($"SubscriptionTypes_{nameof(UpdateSubscriptionType)}");

        group.MapDelete("{id}", DeleteSubscriptionType)
            .WithName($"SubscriptionTypes_{nameof(DeleteSubscriptionType)}");

        return app;
    }

    private static async Task<Ok<PagedResult<SubscriptionTypeDto>>> GetSubscriptionTypes(string organizationId, int page = 1, int pageSize = 10, string? searchTerm = null, string? sortBy = null, SortDirection? sortDirection = null, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new GetSubscriptionTypesQuery(organizationId, page, pageSize, searchTerm, sortBy, sortDirection), cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<SubscriptionTypeDto>, NotFound>> GetSubscriptionTypeById(string organizationId, int id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetSubscriptionTypeQuery(organizationId, id), cancellationToken);

        /*
        if (result.HasError(Errors.Subscriptions.SubscriptionNotFound))
        {
            return TypedResults.NotFound();
        }
        */

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Created<SubscriptionTypeDto>, NotFound>> CreateSubscriptionType(string organizationId, CreateSubscriptionTypeDto request, IMediator mediator, LinkGenerator linkGenerator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateSubscriptionTypeCommand(organizationId, request.Name, request.Handle, request.Description), cancellationToken);

        /*
        if (result.HasError(Errors.Subscriptions.SubscriptionNotFound))
        {
            return TypedResults.NotFound();
        }

        var subscription = result.GetValue();*/

        var path = linkGenerator.GetPathByName(nameof(GetSubscriptionTypeById), new { id = result.Id });

        return TypedResults.Created(path, result);
    }

    private static async Task<Results<Ok, NotFound>> UpdateSubscriptionType(string organizationId, int id, UpdateSubscriptionTypeDto request, IMediator mediator, LinkGenerator linkGenerator, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateSubscriptionTypeCommand(organizationId, id, request.Name, request.Handle, request.Description), cancellationToken);

        /*
        if (result.HasError(Errors.Subscriptions.SubscriptionNotFound))
        {
            return TypedResults.NotFound();
        }

        var subscription = result.GetValue();*/

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> DeleteSubscriptionType(string organizationId, int id, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        await mediator.Send(new DeleteSubscriptionTypeCommand(organizationId, id), cancellationToken);

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

public record CreateSubscriptionTypeDto(string Name, string Handle, string? Description);

public record UpdateSubscriptionTypeDto(string Name, string Handle, string? Description);