namespace YourBrand.Sales.API.Features.OrderManagement.Domain;

public static class Errors
{
    public static class ProductPriceLists
    {
        public static readonly Error ProductPriceListNotFound = new Error(nameof(ProductPriceListNotFound), "ProductPriceListNotFound not found", string.Empty);
    }

    public static class ProductPrice
    {
        public static readonly Error ProductPriceNotFound = new Error(nameof(ProductPriceNotFound), "ProductPrice not found", string.Empty);
    }

    public static class Orders
    {
        public static readonly Error OrderNotFound = new Error(nameof(OrderNotFound), "Order not found", string.Empty);

        public static readonly Error OrderItemNotFound = new Error(nameof(OrderItemNotFound), "Order item not found", string.Empty);
    }

    public static class Users
    {
        public static readonly Error UserNotFound = new Error(nameof(UserNotFound), "User not found", string.Empty);
    }

    public static class Organizations
    {
        public static readonly Error OrganizationNotFound = new Error(nameof(OrganizationNotFound), "Organization not found", string.Empty);
    }
}