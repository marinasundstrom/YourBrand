namespace YourBrand.Sales.Features.OrderManagement.Orders;

public static class WebApplicationExtensions
{
    public static WebApplication MapHubsForApp(this WebApplication app)
    {
        app.MapHub<OrdersHub>("/hubs/orders");

        return app;
    }
}