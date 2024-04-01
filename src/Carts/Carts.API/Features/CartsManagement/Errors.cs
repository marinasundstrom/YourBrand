namespace YourBrand.Carts.API.Features.CartsManagement.Requests;

public static class Errors
{
    public readonly static Error CartNotFound = new("cart-not-found", "Cart not found", "");

    public readonly static Error CartItemNotFound = new("cart-not-found", "Cart not found", "");
}