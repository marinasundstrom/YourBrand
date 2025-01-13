using MassTransit;

using YourBrand.Tenancy;

namespace YourBrand.StoreFront.API.Features.Cart;

public sealed class MassTransitCartsClient(
    IRequestClient<Carts.Contracts.GetCartById> getCartByIdClient,
    IRequestClient<Carts.Contracts.AddCartItem> addCartItemClient,
    IRequestClient<Carts.Contracts.UpdateCartItemPrice> updateCartItemPriceClient,
    IRequestClient<Carts.Contracts.UpdateCartItemQuantity> updateCartItemQuantityClient,
    IRequestClient<Carts.Contracts.UpdateCartItemData> updateCartItemDataClient,
    IRequestClient<Carts.Contracts.RemoveCartItem> removeCartItemClient,
    IRequestClient<Carts.Contracts.GetCartByTag> getCartByTagClient,
    IRequestClient<Carts.Contracts.CreateCart> createCartClient,
    ITenantContext tenantContext,
    ILogger<MassTransitCartsClient> logger)
{
    public async Task<Cart> GetCartById(string cartId, CancellationToken cancellationToken = default)
    {
        var response = await getCartByIdClient.GetResponse<Carts.Contracts.GetCartByIdResponse>(
            new Carts.Contracts.GetCartById { Id = cartId }, cancellationToken);

        return response.Message.Cart.Map();
    }

    public async Task<Cart> GetCartByTag(string tag, CancellationToken cancellationToken = default)
    {
        logger.LogError("GetCartByTag: {TenantId}", tenantContext.TenantId);
        logger.LogError("Tag: {TenantId}", tag);

        var response = await getCartByTagClient.GetResponse<Carts.Contracts.GetCartByTagResponse>(
            new Carts.Contracts.GetCartByTag { Tag = tag }, cancellationToken);

        return response.Message.Cart.Map();
    }

    public async Task<Cart> CreateCart(string tag, CancellationToken cancellationToken = default)
    {
        var response = await createCartClient.GetResponse<Carts.Contracts.CreateCartResponse>(
            new Carts.Contracts.CreateCart { Tag = tag }, cancellationToken);

        return response.Message.Cart.Map();
    }

    public async Task<CartItem> AddCartItem(string cartId, string name, string? image, long? productId, string productHandle, string description, decimal price, double? vatRate, decimal? regularPrice, double? discountRate, int quantity, string? data, CancellationToken cancellationToken = default)
    {
        var request2 = new Carts.Contracts.AddCartItem
        {
            CartId = cartId,
            Name = name,
            Image = image,
            ProductId = productId,
            ProductHandle = productHandle,
            Description = description,
            Price = price,
            VatRate = vatRate,
            RegularPrice = regularPrice,
            DiscountRate = discountRate,
            Quantity = quantity,
            Data = data
        };

        var response = await addCartItemClient.GetResponse<Carts.Contracts.AddCartItemResponse>(request2, cancellationToken);
        return response.Message.CartItem.Map();
    }

    public async Task<CartItem> UpdateCartItemPrice(string cartId, string cartItemId, decimal price, CancellationToken cancellationToken = default)
    {
        var request2 = new Carts.Contracts.UpdateCartItemPrice
        {
            CartId = cartId,
            CartItemId = cartItemId,
            Price = price
        };

        var response = await updateCartItemPriceClient.GetResponse<Carts.Contracts.UpdateCartItemPriceResponse>(request2, cancellationToken);
        return response.Message.CartItem.Map();
    }


    public async Task<CartItem> UpdateCartItemQuantity(string cartId, string cartItemId, int quantity, CancellationToken cancellationToken = default)
    {
        var request2 = new Carts.Contracts.UpdateCartItemQuantity
        {
            CartId = cartId,
            CartItemId = cartItemId,
            Quantity = quantity
        };

        var response = await updateCartItemQuantityClient.GetResponse<Carts.Contracts.UpdateCartItemQuantityResponse>(request2, cancellationToken);
        return response.Message.CartItem.Map();
    }

    public async Task<CartItem> UpdateCartItemData(string cartId, string cartItemId, string? data, CancellationToken cancellationToken = default)
    {
        var request2 = new Carts.Contracts.UpdateCartItemData
        {
            CartId = cartId,
            CartItemId = cartItemId,
            Data = data
        };

        var response = await updateCartItemDataClient.GetResponse<Carts.Contracts.UpdateCartItemDataResponse>(request2, cancellationToken);
        return response.Message.CartItem.Map();
    }

    public async Task RemoveCartItem(string cartId, string cartItemId, CancellationToken cancellationToken = default)
    {
        var request2 = new Carts.Contracts.RemoveCartItem
        {
            CartId = cartId,
            CartItemId = cartItemId
        };

        var response = await removeCartItemClient.GetResponse<Carts.Contracts.RemoveCartItemResponse>(request2, cancellationToken);
    }

    public async Task ClearCart(string cartId, CancellationToken cancellationToken = default)
    {
        var request2 = new Carts.Contracts.ClearCart
        {
            CartId = cartId
        };

        var response = await removeCartItemClient.GetResponse<Carts.Contracts.ClearCartResponse>(request2, cancellationToken);
    }
}

public static class Mapper
{
    public static Cart Map(this Carts.Contracts.Cart cart)
        => new(cart.Id!, cart.Tag!, cart.Items.Select(cartItem => cartItem.Map()));

    public static CartItem Map(this Carts.Contracts.CartItem cartItem)
        => new(cartItem.Id!, cartItem.Name!, cartItem.Image!, cartItem.ProductId, cartItem.ProductHandle, cartItem.Description!, cartItem.Price, cartItem.VatRate, cartItem.RegularPrice, cartItem.DiscountRate, (int)cartItem.Quantity, cartItem.Data);
}