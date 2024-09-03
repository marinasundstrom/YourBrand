using System.Text.Json;

using Microsoft.AspNetCore.Http.HttpResults;

using YourBrand.Catalog;

namespace YourBrand.StoreFront.API.Features.Cart;

public static class Endpoints
{
    public static IEndpointRouteBuilder MapCartEndpoints(this IEndpointRouteBuilder app)
    {
        string GetCartsExpire20 = nameof(GetCartsExpire20);

        var versionedApi = app.NewVersionedApi("Cart");

        var cartGroup = versionedApi.MapGroup("/v{version:apiVersion}/cart")
            .WithTags("Cart")
            .HasApiVersion(ApiVersions.V1)
            .WithOpenApi()
            .RequireCors();

        cartGroup.MapGet("/", GetCart)
            .WithName($"Cart_{nameof(GetCart)}");

        cartGroup.MapPost("/items", AddCartItem)
            .WithName($"Cart_{nameof(AddCartItem)}");

        cartGroup.MapPut("/items/{cartItemId}/quantity", UpdateCartItemQuantity)
            .WithName($"Cart_{nameof(UpdateCartItemQuantity)}");

        cartGroup.MapPut("/items/{cartItemId}/data", UpdateCartItemData)
            .WithName($"Cart_{nameof(UpdateCartItemData)}");

        cartGroup.MapDelete("/items/{cartItemId}", RemoveCartItem)
            .WithName($"Cart_{nameof(RemoveCartItem)}");

        cartGroup.MapDelete("/items/", ClearCart)
            .WithName($"Cart_{nameof(ClearCart)}");

        return app;
    }

    private static async Task<Results<Ok<Features.Cart.Cart>, NotFound>> GetCart(MassTransitCartsClient cartsClient, IUserContext userContext, CancellationToken cancellationToken)
    {
        var customerId = userContext.CustomerNo;
        var clientId = userContext.ClientId;

        string tag = customerId is null ? $"cart-{clientId}" : $"cart-{customerId}";

        Cart? cart = null;

        try
        {
            cart = await cartsClient.GetCartByTag(tag, cancellationToken);
        }
        catch
        {
            cart = await cartsClient.CreateCart(tag, cancellationToken);
        }

        return cart is not null ? TypedResults.Ok(cart) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<CartItem>, NotFound>> AddCartItem(AddCartItemRequest request, MassTransitCartsClient cartsClient, IConfiguration configuration, IProductsClient productsClient, IUserContext userContext, CancellationToken cancellationToken)
    {
        var customerId = userContext.CustomerNo;
        var clientId = userContext.ClientId;

        string tag = customerId is null ? $"cart-{clientId}" : $"cart-{customerId}";

        var product = await productsClient.GetProductByIdAsync(configuration["OrganizationId"]!, request.ProductId.ToString()!, cancellationToken);

        string? data = request.Data;

        if (string.IsNullOrEmpty(data))
        {
            var dataArray = product.Options.Select(x =>
            {
                return new Option
                {
                    Id = x.Option.Id,
                    Name = x.Option.Name,
                    OptionType = (int)x.Option.OptionType,
                    ProductId = x.Option.Sku,
                    Price = x.Option.Price,
                    IsSelected = x.Option.IsSelected,
                    SelectedValueId = x.Option.DefaultValue?.Id,
                    NumericalValue = x.Option.DefaultNumericalValue,
                    TextValue = x.Option.DefaultTextValue
                };
            });

            data = JsonSerializer.Serialize(dataArray, new JsonSerializerOptions
            {
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
            });
        }

        PriceCalculator priceCalculator = new PriceCalculator();
        var (price, regularPrice) = priceCalculator.CalculatePrice(product, data);

        var cart = await cartsClient.GetCartByTag(tag, cancellationToken);

        var cartItem = await cartsClient.AddCartItem(
            cart.Id, product.Name, product.Image?.Url, request.ProductId, product.Handle, product.Description, price, product.VatRate, regularPrice, product.DiscountRate, request.Quantity, data, cancellationToken);

        return cartItem is not null ? TypedResults.Ok(cartItem) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<CartItem>, NotFound>> UpdateCartItemQuantity(string cartItemId, UpdateCartItemQuantityRequest request, MassTransitCartsClient cartsClient, IUserContext userContext, CancellationToken cancellationToken)
    {
        if (request.Quantity <= 0) throw new ArgumentException("Invalid quantity", nameof(request));

        var customerId = userContext.CustomerNo;
        var clientId = userContext.ClientId;

        string tag = customerId is null ? $"cart-{clientId}" : $"cart-{customerId}";

        var cart = await cartsClient.GetCartByTag(tag, cancellationToken);

        var cartItem = await cartsClient.UpdateCartItemQuantity(cart.Id, cartItemId, request.Quantity, cancellationToken);

        return cartItem is not null ? TypedResults.Ok(cartItem) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<CartItem>, NotFound>> UpdateCartItemData(string cartItemId, UpdateCartItemDataRequest request, MassTransitCartsClient cartsClient, IConfiguration configuration, IProductsClient productsClient, IUserContext userContext, CancellationToken cancellationToken)
    {
        var customerId = userContext.CustomerNo;
        var clientId = userContext.ClientId;

        string tag = customerId is null ? $"cart-{clientId}" : $"cart-{customerId}";

        var cart = await cartsClient.GetCartByTag(tag, cancellationToken);
        var cartItem = cart.Items.First(x => x.Id == cartItemId);

        var product = await productsClient.GetProductByIdAsync(configuration["OrganizationId"]!, cartItem.ProductId.ToString()!, cancellationToken);

        PriceCalculator priceCalculator = new PriceCalculator();
        var (price, regularPrice) = priceCalculator.CalculatePrice(product, request.Data!);

        cartItem = await cartsClient.UpdateCartItemPrice(cart.Id, cartItemId, price, cancellationToken);
        cartItem = await cartsClient.UpdateCartItemData(cart.Id, cartItemId, request.Data, cancellationToken);

        return cartItem is not null ? TypedResults.Ok(cartItem) : TypedResults.NotFound();
    }

    private static async Task<Results<Ok, NotFound>> RemoveCartItem(string cartItemId, MassTransitCartsClient cartsClient, IUserContext userContext, CancellationToken cancellationToken)
    {
        var customerId = userContext.CustomerNo;
        var clientId = userContext.ClientId;

        string tag = customerId is null ? $"cart-{clientId}" : $"cart-{customerId}";

        var cart = await cartsClient.GetCartByTag(tag, cancellationToken);

        await cartsClient.RemoveCartItem(cart.Id, cartItemId, cancellationToken);

        return TypedResults.Ok();
    }

    private static async Task<Results<Ok, NotFound>> ClearCart(MassTransitCartsClient cartsClient, IUserContext userContext, CancellationToken cancellationToken)
    {
        var customerId = userContext.CustomerNo;
        var clientId = userContext.ClientId;

        string tag = customerId is null ? $"cart-{clientId}" : $"cart-{customerId}";

        var cart = await cartsClient.GetCartById(tag, cancellationToken);

        await cartsClient.ClearCart(cart.Id, cancellationToken);

        return TypedResults.Ok();
    }
}

public sealed record AddCartItemRequest(long? ProductId, int Quantity, string? Data);

public sealed record UpdateCartItemQuantityRequest(int Quantity);

public sealed record UpdateCartItemDataRequest(string? Data);

public sealed class Cart(string id, string name, IEnumerable<CartItem> items)
{
    public string Id { get; set; } = id;

    public string Tag { get; set; } = name;

    public IEnumerable<CartItem> Items { get; set; } = items;
}

public sealed class CartItem(string id, string name, string? image, long? productId, string? productHandle, string description, decimal price, double? vatRate, decimal? regularPrice, double? discountRate, int quantity, string? data)
{
    public string Id { get; set; } = id;

    public string Name { get; set; } = name;

    public string? Image { get; set; } = image;

    public long? ProductId { get; set; } = productId;

    public string? ProductHandle { get; set; } = productHandle;

    public string Description { get; set; } = description;

    public decimal Price { get; set; } = price;

    public double? VatRate { get; set; } = vatRate;

    public decimal? RegularPrice { get; set; } = regularPrice;

    public double? DiscountRate { get; set; } = discountRate;

    public int Quantity { get; set; } = quantity;

    public decimal Total => Price * Quantity;

    public string? Data { get; set; } = data;
}

public class Option
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public int OptionType { get; set; }

    public string? ProductId { get; set; }

    public decimal? Price { get; set; }

    public string? TextValue { get; set; }

    public int? NumericalValue { get; set; }

    public bool? IsSelected { get; set; }

    public string? SelectedValueId { get; set; }
}