using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Sales.Features.OrderManagement.Orders.Commands;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Orders.Queries;
using YourBrand.Sales.Features.OrderManagement.Orders.Statuses.Commands;
using YourBrand.Sales.Features.OrderManagement.Orders.Statuses.Queries;
using YourBrand.Sales.Models;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Statuses;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapOrderStatusEndpoints(this IEndpointRouteBuilder app)
    {
        string GetOrdersExpire20 = nameof(GetOrdersExpire20);

        var versionedApi = app.NewVersionedApi("OrderStatuses");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/orders/statuses")
            .WithTags("OrderStatuses")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi();

        group.MapGet("/", GetOrderStatuses)
            .WithName($"OrderStatuses_{nameof(GetOrderStatuses)}");

        group.MapGet("/{id}", GetOrderStatusById)
            .WithName($"OrderStatuses_{nameof(GetOrderStatusById)}");

        group.MapPost("/", CreateOrderStatus)
            .WithName($"OrderStatuses_{nameof(CreateOrder)}");

        group.MapPost("{id}", UpdateOrderStatus)
            .WithName($"OrderStatuses_{nameof(UpdateOrderStatus)}");

        group.MapDelete("{id}", DeleteOrderStatus)
            .WithName($"OrderStatuses_{nameof(DeleteOrderStatus)}");

        return app;
    }

    private static async Task<Ok<PagedResult<OrderStatusDto>>> GetOrderStatuses(string organizationId, int page = 1, int pageSize = 10, string? searchTerm = null, string? sortBy = null, SortDirection? sortDirection = null, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new GetOrderStatusesQuery(organizationId, page, pageSize, searchTerm, sortBy, sortDirection), cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<OrderStatusDto>, NotFound>> GetOrderStatusById(string organizationId, int id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrderStatusQuery(organizationId, id), cancellationToken);

        /*
        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }
        */

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Created<OrderStatusDto>, NotFound>> CreateOrderStatus(string organizationId, CreateOrderStatusDto request, IMediator mediator, LinkGenerator linkGenerator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateOrderStatusCommand(organizationId, request.Name, request.Handle, request.Description), cancellationToken);

        /*
        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        var order = result.GetValue();*/

        var path = linkGenerator.GetPathByName(nameof(GetOrderById), new { id = result.Id });

        return TypedResults.Created(path, result);
    }

    private static async Task<Results<Ok, NotFound>> UpdateOrderStatus(string organizationId, int id, UpdateOrderStatusDto request, IMediator mediator, LinkGenerator linkGenerator, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateOrderStatusCommand(organizationId, id, request.Name, request.Handle, request.Description), cancellationToken);

        /*
        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        var order = result.GetValue();*/

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> DeleteOrderStatus(string organizationId, int id, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        await mediator.Send(new DeleteOrderStatusCommand(organizationId, id), cancellationToken);

        /*
        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        if (result.HasError(Errors.Orders.OrderItemNotFound))
        {
            return TypedResults.NotFound();
        }
        */

        return TypedResults.Ok();
    }
}

public record CreateOrderStatusDto(string Name, string Handle, string? Description);

public record UpdateOrderStatusDto(string Name, string Handle, string? Description);