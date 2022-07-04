using MediatR;

using YourBrand.Orders.Application.Orders;
using YourBrand.Orders.Application.Subscriptions;

namespace YourBrand.Orders.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(typeof(ServiceCollectionExtensions));
              
        services.AddOrders();
        services.AddSubscriptions();

        return services;
    }
}