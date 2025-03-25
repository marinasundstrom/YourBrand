using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Sales.Features.OrderManagement.Orders.Commands;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Orders.Items;
using YourBrand.Sales.Features.OrderManagement.Orders.Items.Commands;
using YourBrand.Sales.Features.OrderManagement.Orders.Queries;
using YourBrand.Sales.Features.SubscriptionManagement;
using YourBrand.Sales.Models;

namespace YourBrand.Sales.Features.OrderManagement.Orders;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        string GetOrdersExpire20 = nameof(GetOrdersExpire20);

        var versionedApi = app.NewVersionedApi("Orders");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/orders")
            .WithTags("Orders")
            .HasApiVersion(ApiVersions.V1)
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("/", GetOrders)
            .WithName($"Orders_{nameof(GetOrders)}")
            .CacheOutput(GetOrdersExpire20);

        group.MapGet("/{id}", GetOrderById)
            .WithName($"Orders_{nameof(GetOrderById)}");

        group.MapGet("/getByNo/{orderNo}", GetOrderByNo)
            .WithName($"Orders_{nameof(GetOrderByNo)}");

        group.MapPost("/", CreateOrder)
            .WithName($"Orders_{nameof(CreateOrder)}");

        group.MapPost("/draft", CreateDraftOrder)
            .WithName($"Orders_{nameof(CreateDraftOrder)}");

        group.MapDelete("{id}", DeleteOrder)
            .WithName($"Orders_{nameof(DeleteOrder)}");

        group.MapPost("{id}/items", AddOrderItem)
            .WithName($"Orders_{nameof(AddOrderItem)}");

        group.MapGet("{id}/items/{itemId}", GetOrderItemById)
            .WithName($"Orders_{nameof(GetOrderItemById)}");

        group.MapPut("{id}/items/{itemId}", UpdateOrderItem)
            .WithName($"Orders_{nameof(UpdateOrderItem)}");

        group.MapPut("{id}/items/{itemId}/quantity", UpdateOrderItemQuantity)
            .WithName($"Orders_{nameof(UpdateOrderItemQuantity)}");

        group.MapPut("{id}/status", UpdateStatus)
            .WithName($"Orders_{nameof(UpdateStatus)}");

        group.MapPut("{id}/customer", SetCustomer)
            .WithName($"Orders_{nameof(SetCustomer)}");

        group.MapPut("{id}/assignee", UpdateAssignedUser)
            .WithName($"Orders_{nameof(UpdateAssignedUser)}");

        group.MapPut("{id}/billingDetails", UpdateBillingDetails)
            .WithName($"Orders_{nameof(UpdateBillingDetails)}");

        group.MapPut("{id}/shippingDetails", UpdateShippingDetails)
            .WithName($"Orders_{nameof(UpdateShippingDetails)}");

        /*

    group.MapPut("{orderId}/items/{id}/price", UpdateOrderItemPrice)
        .WithName($"Orders_{nameof(UpdateOrderItemPrice)}");
        
    group.MapPut("{orderId}/items/{id}/data", UpdateOrderItemData)
        .WithName($"Orders_{nameof(UpdateOrderItemData)}");

        */

        group.MapDelete("{id}/items/{itemId}", RemoveOrderItem)
            .WithName($"Orders_{nameof(RemoveOrderItem)}");

        group.MapPost("{id}/items/{orderItemId}/options", AddOrderItemOption)
            .WithName($"Orders_{nameof(AddOrderItemOption)}");

        group.MapPut("{id}/items/{itemId}/options/{optionId}", UpdateOrderItemOption)
            .WithName($"Orders_{nameof(UpdateOrderItemOption)}");

        group.MapDelete("{id}/items/{itemId}/options/{optionId}", RemoveOrderItemOption)
            .WithName($"Orders_{nameof(RemoveOrderItemOption)}");


        group.MapPost("{id}/subscription/activate", ActivateSubscriptionOrder)
                    .WithName($"Orders_{nameof(ActivateSubscriptionOrder)}")
                    .AllowAnonymous();

        return app;
    }

    private static async Task<Results<Created<OrderItemOptionDto>, NotFound>> AddOrderItemOption(string organizationId, string id, string itemId, AddOrderItemOptionRequest request, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new Items.Options.CreateOrderItemOption(organizationId, id, itemId, request.Description, request.ProductId, request.ItemId, request.Price, request.Discount), cancellationToken);
        return TypedResults.Created("", result.GetValue());
    }

    private static async Task<Results<Ok<OrderItemOptionDto>, NotFound>> UpdateOrderItemOption(string organizationId, string id, string itemId, string optionId, UpdateOrderItemOptionRequest request, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new Items.Options.UpdateOrderItemOption(organizationId, id, itemId, optionId, request.Description, request.ProductId, request.ItemId, request.Price, request.Discount), cancellationToken);
        return TypedResults.Ok(result.GetValue());
    }

    private static async Task<Results<Ok, NotFound>> RemoveOrderItemOption(string organizationId, string id, string itemId, string optionId, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new Items.Options.RemoveOrderItemOption(organizationId, id, itemId, optionId), cancellationToken);
        return TypedResults.Ok();
    }

    private static async Task<Ok<PagedResult<OrderDto>>> GetOrders(string organizationId, int[]? types, int[]? status, string? customerId, string? ssn, string? assigneeId, Guid? subscriptionId, DateTimeOffset? fromDate = null, DateTimeOffset? toDate = null, DateTimeOffset? plannedFromDate = null, DateTimeOffset? plannedToDate = null, int page = 1, int pageSize = 10, string? sortBy = null, SortDirection? sortDirection = null, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new GetOrders(organizationId, types, status, customerId, ssn, assigneeId, subscriptionId, fromDate, toDate, plannedFromDate, plannedToDate, page, pageSize, sortBy, sortDirection), cancellationToken);
        return TypedResults.Ok(result.GetValue());
    }

    private static async Task<Results<Ok<OrderDto>, NotFound>> GetOrderById(string organizationId, string id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrderById(organizationId, id), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.GetValue());
    }

    private static async Task<Results<Ok<OrderDto>, NotFound>> GetOrderByNo(string organizationId, int orderNo, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOrderByNo(organizationId, orderNo), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.GetValue());
    }

    private static async Task<Results<Created<OrderDto>, NotFound>> CreateOrder(string organizationId, CreateOrderRequest request, IMediator mediator, LinkGenerator linkGenerator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateOrder(organizationId, request.Status, request.Customer, request.BillingDetails, request.ShippingDetails, request.Items), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        var order = result.GetValue();

        var path = linkGenerator.GetPathByName(nameof(GetOrderById), new { id = order.Id });

        return TypedResults.Created(path, order);
    }

    private static async Task<Results<Created<OrderDto>, NotFound>> CreateDraftOrder(string organizationId, CreateDraftOrderRequest request, IMediator mediator, LinkGenerator linkGenerator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateDraftOrder(organizationId), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        var order = result.GetValue();

        var path = linkGenerator.GetPathByName(nameof(GetOrderById), new { id = order.Id });

        return TypedResults.Created(path, order);
    }

    private static async Task<Results<Ok, NotFound>> DeleteOrder(string organizationId, string id, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new DeleteOrder(organizationId, id), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok();
    }

    private static async Task<Results<Created<OrderItemDto>, NotFound>> AddOrderItem(string organizationId, string id, AddOrderItemRequest request, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new CreateOrderItem(organizationId, id, request.Description, request.ItemId, request.SubscriptionPlanId, request.Quantity, request.Unit, request.UnitPrice, request.RegularPrice, request.VatRate, request.Discount, request.Notes), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        var orderItem = result.GetValue();

        var path = linkGenerator.GetPathByName(nameof(GetOrderItemById), new { id = orderItem.Id });

        return TypedResults.Created(path, orderItem);
    }

    private static async Task<Results<Created<OrderItemDto>, NotFound>> UpdateOrderItem(string organizationId, string id, string itemId, UpdateOrderItemRequest request, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new UpdateOrderItem(organizationId, id, itemId, request.Description, request.ItemId, request.SubscriptionPlanId, request.Quantity, request.Unit, request.UnitPrice, request.RegularPrice, request.VatRate, request.Discount, request.Notes), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        var orderItem = result.GetValue();

        var path = linkGenerator.GetPathByName(nameof(GetOrderItemById), new { id = orderItem.Id });

        return TypedResults.Created(path, orderItem);
    }

    private static async Task<Results<Ok<OrderItemDto>, NotFound>> UpdateOrderItemQuantity(string organizationId, string id, string itemId, UpdateOrderItemQuantityRequest request, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new UpdateOrderItemQuantity(organizationId, id, itemId, request.Quantity), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        var orderItem = result.GetValue();

        return TypedResults.Ok(orderItem);
    }

    private static async Task<Results<Ok<OrderItemDto>, NotFound>> GetOrderItemById(string organizationId, string id, string itemId, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new GetOrderItemById(organizationId, id, itemId), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        if (result.HasError(Errors.Orders.OrderItemNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.GetValue());
    }

    private static async Task<Results<Ok, NotFound>> UpdateStatus(string organizationId, string id, int status, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new UpdateStatus(organizationId, id, status), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> SetCustomer(string organizationId, string id, SetCustomerDto customer, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new SetCustomer(organizationId, id, customer.Id, customer.Name), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> UpdateAssignedUser(string organizationId, string id, string? userId, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new UpdateAssignedUser(organizationId, id, userId), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> UpdateBillingDetails(string organizationId, string id, BillingDetailsDto billingDetails, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new UpdateBillingDetails(organizationId, id, billingDetails), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> UpdateShippingDetails(string organizationId, string id, ShippingDetailsDto shippingDetails, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new UpdateShippingDetails(organizationId, id, shippingDetails), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> ActivateSubscriptionOrder(string organizationId, string id, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        await mediator.Send(new ActivateSubscriptionOrder(organizationId, id), cancellationToken);

        return TypedResults.Ok();
    }

    /*

    private static async Task<Results<Ok<OrderItemDto>, NotFound>> UpdateOrderItemPrice(string orderId, string id, decimal price, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new UpdateOrderItemPrice(orderId, id, price), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        if (result.HasError(Errors.Orders.OrderItemNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.GetValue());
    }

    private static async Task<Results<Ok<OrderItemDto>, NotFound>> UpdateOrderItemQuantity(string orderId, string id, int quantity, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new UpdateOrderItemQuantity(orderId, id, quantity), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        if (result.HasError(Errors.Orders.OrderItemNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.GetValue());
    }

    private static async Task<Results<Ok<OrderItemDto>, NotFound>> UpdateOrderItemData(string orderId, string id, string data, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new UpdateOrderItemData(orderId, id, data), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        if (result.HasError(Errors.Orders.OrderItemNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.GetValue());
    }

    */

    private static async Task<Results<Ok, NotFound>> RemoveOrderItem(string organizationId, string id, string itemId, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new RemoveOrderItem(organizationId, id, itemId), cancellationToken);

        if (result.HasError(Errors.Orders.OrderNotFound))
        {
            return TypedResults.NotFound();
        }

        if (result.HasError(Errors.Orders.OrderItemNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok();
    }
}

public record AddOrderItemOptionRequest(string Description, string? ProductId, string? ItemId, decimal? Price, decimal? Discount);

public record UpdateOrderItemOptionRequest(string Description, string? ProductId, string? ItemId, decimal? Price, decimal? Discount);