using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Features.OrderManagement.Orders;
using YourBrand.Sales.Features.SubscriptionManagement;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public static class Mappings
{
    public static SubscriptionDto ToDto(this Subscription subscription)
    {
        return new SubscriptionDto(subscription.Id, subscription.SubscriptionNo, subscription.Type.ToDto(), ToShortDto(subscription.Plan!), subscription.Status.ToDto(), subscription.StartDate, subscription.EndDate, subscription.Order?.ToShortDto(), subscription.OrderItemId, null, default, subscription.Schedule.ToDto());
    }

    public static SubscriptionPlanDto ToDto(this SubscriptionPlan subscriptionPlan)
    {
        return new SubscriptionPlanDto(subscriptionPlan.Id, subscriptionPlan.Name, subscriptionPlan.ItemId, subscriptionPlan.Schedule.ToDto(), default, default, null, subscriptionPlan.RenewalOption);
    }

    public static SubscriptionSchedule ToDto(this SubscriptionSchedule schedule)
    {
        return new SubscriptionSchedule
        {
            Frequency = schedule.Frequency,
            EveryDays = schedule.EveryDays,
            EveryWeeks = schedule.EveryWeeks,
            OnWeekDays = schedule.OnWeekDays,
            EveryMonths = schedule.EveryMonths,
            EveryYears = schedule.EveryYears,
            OnDay = schedule.OnDay,
            OnDayOfWeek = schedule.OnDayOfWeek,
            InMonth = schedule.InMonth,
            StartTime = schedule.StartTime,
            Duration = schedule.Duration,
        };
    }
    public static SubscriptionPlanShortDto ToShortDto(this SubscriptionPlan subscriptionPlan)
    {
        return new SubscriptionPlanShortDto(subscriptionPlan.Id, subscriptionPlan.Name, subscriptionPlan.ItemId);
    }

    public static SubscriptionTypeDto ToDto(this SubscriptionType subscriptionType) => new(subscriptionType.Id, subscriptionType.Name, subscriptionType.Handle, subscriptionType.Description);

    public static SubscriptionStatusDto ToDto(this SubscriptionStatus subscriptionStatus) => new(subscriptionStatus.Id, subscriptionStatus.Name, subscriptionStatus.Handle, subscriptionStatus.Description);

}