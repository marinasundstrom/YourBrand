namespace BlazorApp;

public static class PageRoutes
{
    public const string Home = "/";
    public const string Category = "/categories/{*path}";
    public const string Products = "/products";
    public const string Product = "/products/{id}";
    public const string ProductVariant = "/products/{id}/{variantId}";
    public const string Basket = "/basket";
    public const string Checkout = "/checkout";
    public const string OrderConfirmation = "/order/confirmation";
    public const string Register = "/authentication/register";
    public const string AuthBegin = "/authentication/begin";
    public const string AuthPending = "/authentication/pending";
    public const string AuthConfirm = "/authentication/confirm";
    public const string NotCustomer = "/authentication/not-customer";
    public const string Login = "/login";
    public const string AuthLogout = "/authentication/logout";
    public const string Profile = "/authentication/profile";
    public const string MyPages = "/mypage";
    public const string Error = "/error/{code}";
}