namespace YourBrand.Sales.Contracts;

public sealed record GetCarts
{
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 2;
}

public sealed record GetCartById
{
    public string Id { get; init; }
}

public sealed record GetCartByIdResponse
{
    public Cart Cart { get; init; }
}

public sealed record GetCartByTag
{
    public string Tag { get; init; }
}

public sealed record GetCartByTagResponse
{
    public Cart Cart { get; init; }
}

public sealed record CreateCart
{
    public string Tag { get; init; }
}

public sealed record CreateCartResponse
{
    public Cart Cart { get; init; }
}


public sealed record AddCartItem
{
    public string CartId { get; init; }
    public string Name { get; init; }
    public string? Image { get; init; }
    public long? ProductId { get; init; }
    public string? ProductHandle { get; init; }
    public string Description { get; init; }
    public decimal Price { get; init; }
    public decimal? RegularPrice { get; init; }
    public int Quantity { get; init; } = 1;
    public string? Data { get; set; }
}

public sealed record AddCartItemResponse
{
    public CartItem CartItem { get; init; }
}

public sealed record UpdateCartItemPrice
{
    public string CartId { get; init; }
    public string CartItemId { get; init; }
    public decimal Price { get; init; }
}

public sealed record UpdateCartItemPriceResponse
{
    public CartItem CartItem { get; init; }
}

public sealed record UpdateCartItemQuantity
{
    public string CartId { get; init; }
    public string CartItemId { get; init; }
    public int Quantity { get; init; }
}

public sealed record UpdateCartItemQuantityResponse
{
    public CartItem CartItem { get; init; }
}

public sealed record UpdateCartItemData
{
    public string CartId { get; init; }
    public string CartItemId { get; init; }
    public string? Data { get; init; }
}

public sealed record UpdateCartItemDataResponse
{
    public CartItem CartItem { get; init; }
}

public sealed record RemoveCartItem
{
    public string CartId { get; init; }
    public string CartItemId { get; init; }
}

public sealed record RemoveCartItemResponse
{

}

public sealed record GetCartItemById
{
    public string CartId { get; init; }
    public string CartItemId { get; init; }
}

public sealed record GetCartItemByIdResponse
{
    public CartItem CartItem { get; init; }
}