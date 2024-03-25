using System.Collections.Generic;
using System.Linq;

using YourBrand.Sales.Features.Subscriptions;
using YourBrand.Sales.Contracts;
using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.API.Features.OrderManagement.Orders;

namespace YourBrand.Sales.Features.Subscriptions;

public static class Mappings
{
    public static SubscriptionDto ToDto(this Subscription subscription)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            Plan = ToShortDto(subscription.SubscriptionPlan!),
            StartDate = subscription.StartDate,
            EndDate = subscription.EndDate,
            Order = subscription.Order?.ToShortDto(),
            OrderItemId = subscription.OrderItemId
        };
    }

    public static SubscriptionPlanDto ToDto(this SubscriptionPlan subscriptionPlan)
    {
        return new SubscriptionPlanDto
        {
            Id = subscriptionPlan.Id,
            Name = subscriptionPlan.Name,
            ProductId = subscriptionPlan.ItemId,
            Price = subscriptionPlan.Price
        };
    }

    public static SubscriptionPlanShortDto ToShortDto(this SubscriptionPlan subscriptionPlan)
    {
        return new SubscriptionPlanShortDto
        {
            Id = subscriptionPlan.Id,
            Name = subscriptionPlan.Name,
            ProductId = subscriptionPlan.ItemId,
            Price = subscriptionPlan.Price
        };
    }
}