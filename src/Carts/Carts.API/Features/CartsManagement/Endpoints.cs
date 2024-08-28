using MediatR;

using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Carts.Domain.Entities;
using YourBrand.Carts.Features.CartsManagement.Requests;

namespace YourBrand.Carts.Features.CartsManagement;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapCartsEndpoints(this IEndpointRouteBuilder app)
    {
        string GetCartsExpire20 = nameof(GetCartsExpire20);

        var versionedApi = app.NewVersionedApi("Carts");

        var group = versionedApi.MapGroup("/v{version:apiVersion}/carts")
            .WithTags("Carts")
            .RequireAuthorization()
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi();

        group.MapGet("/", GetCarts)
            .WithName($"Carts_{nameof(GetCarts)}")
            .CacheOutput(GetCartsExpire20);

        group.MapGet("/GetByTag/{tag}", GetCartByTag)
            .WithName($"Carts_{nameof(GetCartByTag)}");

        group.MapGet("/{id}", GetCartById)
            .WithName($"Carts_{nameof(GetCartById)}");

        group.MapPost("/", CreateCart)
            .WithName($"Carts_{nameof(CreateCart)}");

        group.MapPost("{id}/items", AddCartItem)
            .WithName($"Carts_{nameof(AddCartItem)}");

        group.MapGet("{cartId}/items/{id}", GetCartItemById)
            .WithName($"Carts_{nameof(GetCartItemById)}");

        group.MapPut("{cartId}/items/{id}/price", UpdateCartItemPrice)
            .WithName($"Carts_{nameof(UpdateCartItemPrice)}");

        group.MapPut("{cartId}/items/{id}/quantity", UpdateCartItemQuantity)
            .WithName($"Carts_{nameof(UpdateCartItemQuantity)}");

        group.MapPut("{cartId}/items/{id}/data", UpdateCartItemData)
            .WithName($"Carts_{nameof(UpdateCartItemData)}");

        group.MapDelete("{cartId}/items/{id}", RemoveCartItem)
            .WithName($"Carts_{nameof(RemoveCartItem)}");

        group.MapDelete("{cartId}/items", ClearCart)
            .WithName($"Carts_{nameof(ClearCart)}");

        return app;
    }

    private static async Task<Ok<PagedResult<Cart>>> GetCarts(int page = 1, int pageSize = 10, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new GetCarts(page, pageSize), cancellationToken);
        return TypedResults.Ok(result.GetValue());
    }

    private static async Task<Results<Ok<Cart>, NotFound>> GetCartById(string id, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCartById(id), cancellationToken);

        if (result.HasError(Errors.CartNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.GetValue());
    }

    private static async Task<Results<Ok<Cart>, NotFound>> GetCartByTag(string tag, IMediator mediator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetCartByTag(tag), cancellationToken);

        if (result.HasError(Errors.CartNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.GetValue());
    }

    private static async Task<Results<Created<Cart>, NotFound>> CreateCart(CreateCartRequest request, IMediator mediator, LinkGenerator linkGenerator, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new CreateCart(request.Tag), cancellationToken);

        if (result.HasError(Errors.CartNotFound))
        {
            return TypedResults.NotFound();
        }

        var cart = result.GetValue();

        var path = linkGenerator.GetPathByName(nameof(GetCartById), new { id = cart.Id });

        return TypedResults.Created(path, cart);
    }

    private static async Task<Results<Created<CartItem>, NotFound>> AddCartItem(string id, AddCartItemRequest cartItemRequest, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new AddCartItem(id, cartItemRequest.Name, cartItemRequest.Image, cartItemRequest.ProductId, cartItemRequest.ProductHandle, cartItemRequest.Description, cartItemRequest.Price, cartItemRequest.VatRate, cartItemRequest.RegularPrice, cartItemRequest.DiscountRate, cartItemRequest.Quantity, cartItemRequest.Data), cancellationToken);

        if (result.HasError(Errors.CartNotFound))
        {
            return TypedResults.NotFound();
        }

        var cartItem = result.GetValue();

        var path = linkGenerator.GetPathByName(nameof(GetCartItemById), new { id = cartItem.Id });

        return TypedResults.Created(path, cartItem);
    }

    private static async Task<Results<Ok<CartItem>, NotFound>> GetCartItemById(string cartId, string id, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new GetCartItemById(cartId, id), cancellationToken);

        if (result.HasError(Errors.CartNotFound))
        {
            return TypedResults.NotFound();
        }

        if (result.HasError(Errors.CartItemNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.GetValue());
    }


    private static async Task<Results<Ok<CartItem>, NotFound>> UpdateCartItemPrice(string cartId, string id, decimal price, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new UpdateCartItemPrice(cartId, id, price), cancellationToken);

        if (result.HasError(Errors.CartNotFound))
        {
            return TypedResults.NotFound();
        }

        if (result.HasError(Errors.CartItemNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.GetValue());
    }

    private static async Task<Results<Ok<CartItem>, NotFound>> UpdateCartItemQuantity(string cartId, string id, int quantity, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new UpdateCartItemQuantity(cartId, id, quantity), cancellationToken);

        if (result.HasError(Errors.CartNotFound))
        {
            return TypedResults.NotFound();
        }

        if (result.HasError(Errors.CartItemNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.GetValue());
    }

    private static async Task<Results<Ok<CartItem>, NotFound>> UpdateCartItemData(string cartId, string id, string data, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new UpdateCartItemData(cartId, id, data), cancellationToken);

        if (result.HasError(Errors.CartNotFound))
        {
            return TypedResults.NotFound();
        }

        if (result.HasError(Errors.CartItemNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(result.GetValue());
    }

    private static async Task<Results<Ok, NotFound>> RemoveCartItem(string cartId, string id, IMediator mediator = default!, LinkGenerator linkGenerator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new RemoveCartItem(cartId, id), cancellationToken);

        if (result.HasError(Errors.CartNotFound))
        {
            return TypedResults.NotFound();
        }

        if (result.HasError(Errors.CartItemNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> ClearCart(string cartId, IMediator mediator = default!, CancellationToken cancellationToken = default!)
    {
        var result = await mediator.Send(new ClearCart(cartId), cancellationToken);

        if (result.HasError(Errors.CartNotFound))
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok();
    }

    public sealed record CreateCartRequest(string Tag);

    public sealed record AddCartItemRequest(string Name, string? Image, long? ProductId, string? ProductHandle, string Description, decimal Price, double? VatRate, decimal? RegularPrice, double? DiscountRate, int Quantity, string? Data);
}