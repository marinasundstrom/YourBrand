using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.SubscriptionManagement;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public static class Mappings
{
    public static SubscriptionDto ToDto(this Subscription subscription)
    {
        return new SubscriptionDto
        {
            Id = subscription.Id,
            SubscriptionNo = subscription.SubscriptionNo,
            Plan = ToShortDto(subscription.SubscriptionPlan!),
            Status = subscription.Status.ToDto(),
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
            Price = subscriptionPlan.Price,

            AutoRenew = subscriptionPlan.AutoRenew,
            Recurrence = subscriptionPlan.Recurrence,
            EveryDays = subscriptionPlan.EveryDays,
            EveryWeeks = subscriptionPlan.EveryWeeks,
            OnWeekDays = subscriptionPlan.OnWeekDays,
            EveryMonths = subscriptionPlan.EveryMonths,
            EveryYears = subscriptionPlan.EveryYears,
            OnDay = subscriptionPlan.OnDay,
            OnDayOfWeek = subscriptionPlan.OnDayOfWeek,
            InMonth = subscriptionPlan.InMonth,
            StartTime = subscriptionPlan.StartTime,
            Duration = subscriptionPlan.Duration,
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

    public static SubscriptionTypeDto ToDto(this SubscriptionType subscriptionType) => new(subscriptionType.Id, subscriptionType.Name, subscriptionType.Handle, subscriptionType.Description);

    public static SubscriptionStatusDto ToDto(this SubscriptionStatus subscriptionStatus) => new(subscriptionStatus.Id, subscriptionStatus.Name, subscriptionStatus.Handle, subscriptionStatus.Description);

}