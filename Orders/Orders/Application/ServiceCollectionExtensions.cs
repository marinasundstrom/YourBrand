using MediatR;

using YourBrand.Orders.Application.Orders;
using YourBrand.Orders.Application.Subscriptions;

namespace YourBrand.Orders.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssemblyContaining(typeof(ServiceCollectionExtensions)));
              
        services.AddOrders();
        services.AddSubscriptions();

        return services;
    }
}