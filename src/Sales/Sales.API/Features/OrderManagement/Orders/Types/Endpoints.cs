using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Sales.Features.OrderManagement.Orders.Commands;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Orders.Queries;
using YourBrand.Sales.Features.OrderManagement.Orders.Types.Commands;
using YourBrand.Sales.Features.OrderManagement.Orders.Types.Queries;
using YourBrand.Sales.Models;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Types;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapOrderTypeEndpoints(this IEndpointRouteBuilder app)
    {
        string GetOrdersExpire20 = nameof(GetOrdersExpire20);

        var versionedApi = app.NewVersionedApi("OrderTypes");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/orders/types")
            .WithTags("OrderTypes")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi();

        group.MapGet("/", GetOrderTypes)
            .WithName($"OrderTypes_{nameof(GetOrderTypes)}");

        group.MapGet("/{id}", GetOrderTypeById)
            .WithName($"OrderTypes_{nameof(GetOrderTypeById)}");

        group.MapPost("/", CreateOrderType)
            .WithName($"OrderTypes_{nameof(CreateOrder)}");

        group.MapPost("{id}", UpdateOrderType)
            .WithName($"OrderTypes_{nameof(UpdateOrderType)}");

        group.MapDelete("{id}", DeleteOrderType)
            .WithName($"OrderTypes_{nameof(DeleteOrderType)}");

        return app;
    }

    private static async Task<Ok<PagedResult<OrderTypeDto>>> GetOrderTypes(string organizationId, int page = 1, int pageSize = 10, string? searchTerm = null, string? sortBy = null, SortDirection? sortDirection = null, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new GetOrderTypesQuery(organizationId, page, pageSize, searchTerm, sortBy, sortDirection), cancellationToken);
        return TypedResults.Ok(result);
    }

    private static async Task<Results<Ok<OrderTypeDto>, NotFound>> GetOrderTypeById(string organizationId, int id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrderTypeQuery(organizationId, id), cancellationToken);

        /*
        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }
        */

        return TypedResults.Ok(result);
    }

    private static async Task<Results<Created<OrderTypeDto>, NotFound>> CreateOrderType(string organizationId, CreateOrderTypeDto request, IMediator mediator, LinkGenerator linkGenerator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateOrderTypeCommand(organizationId, request.Name, request.Handle, request.Description), cancellationToken);

        /*
        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        var order = result.GetValue();*/

        var path = linkGenerator.GetPathByName(nameof(GetOrderById), new { id = result.Id });

        return TypedResults.Created(path, result);
    }

    private static async Task<Results<Ok, NotFound>> UpdateOrderType(string organizationId, int id, UpdateOrderTypeDto request, IMediator mediator, LinkGenerator linkGenerator, CancellationToken cancellationToken)
    {
        await mediator.Send(new UpdateOrderTypeCommand(organizationId, id, request.Name, request.Handle, request.Description), cancellationToken);

        /*
        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        var order = result.GetValue();*/

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> DeleteOrderType(string organizationId, int id, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        await mediator.Send(new DeleteOrderTypeCommand(organizationId, id), cancellationToken);

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

public record CreateOrderTypeDto(string Name, string Handle, string? Description);

public record UpdateOrderTypeDto(string Name, string Handle, string? Description);