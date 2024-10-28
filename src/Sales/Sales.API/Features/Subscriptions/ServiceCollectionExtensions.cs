using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Features.Orders;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSubscriptions(this IServiceCollection services)
    {
        services.AddScoped<SubscriptionOrderDateGenerator>()
                .AddScoped<SubscriptionOrderGenerator>()
                .AddScoped<IBillingDateCalculator, DefaultBillingDateCalculator>()
                .AddScoped<IDeliveryDateCalculator, DefaultDeliveryDateCalculator>()
                .AddScoped<OrderFactory>();

        return services;
    }
}