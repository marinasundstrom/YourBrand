using Microsoft.Extensions.DependencyInjection;

namespace YourBrand.Orders.Application.Orders;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOrders(this IServiceCollection services)
    {
        services.AddScoped<OrderFactory>();

        return services;
    }
}