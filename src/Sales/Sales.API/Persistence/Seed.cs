using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Enums;
using YourBrand.Sales.Features.Subscriptions;

namespace YourBrand.Sales.Persistence;

public static class Seed
{
    public static async Task SeedData(SalesContext context, IConfiguration configuration)
    {
        Version1(context);

        await context.SaveChangesAsync();
    }

    private static void Version1(SalesContext context)
    {
        context.OrderStatuses.Add(new OrderStatus("Draft", "draft", string.Empty));

        context.OrderStatuses.Add(new OrderStatus("Open", "open", string.Empty));
        context.OrderStatuses.Add(new OrderStatus("Archived", "archived", string.Empty));
        context.OrderStatuses.Add(new OrderStatus("Canceled", "canceled", string.Empty));

        var subscriptionPlan0 = SubscriptionPlanFactory
                    .CreateWeeklyPlan(1, WeekDays.Tuesday | WeekDays.Thursday, TimeSpan.Parse("16:00"), null)
                    .WithName("Bi-weekly subscription")
                    .WithEndTime(TimeSpan.Parse("17:00"));

        context.SubscriptionPlans.Add(subscriptionPlan0);

        var subscriptionPlan = SubscriptionPlanFactory
                   .CreateMonthlyPlan(1, 1, DayOfWeek.Tuesday, TimeSpan.Parse("10:30"), TimeSpan.Parse("00:30"))
                   .WithName("Monthly subscription 1");

        context.SubscriptionPlans.Add(subscriptionPlan);

        var subscriptionPlan2 = SubscriptionPlanFactory
            .CreateMonthlyPlan(1, 1, DayOfWeek.Tuesday, TimeSpan.Parse("07:30"), null) // , TimeSpan.Parse("00:45"))
            .WithName("Monthly subscription 2");

        context.SubscriptionPlans.Add(subscriptionPlan2);

        var subscriptionPlan3 = SubscriptionPlanFactory
                    .CreateYearlyPlan(1, Month.April, 15, TimeSpan.Parse("14:30"), TimeSpan.Parse("00:30"))
                    .WithName("Yearly subscription 1");

        context.SubscriptionPlans.Add(subscriptionPlan3);

        var subscriptionPlan4 = SubscriptionPlanFactory
                    .CreateYearlyPlan(1, Month.April, 3, DayOfWeek.Thursday, TimeSpan.Parse("09:00"), TimeSpan.Parse("00:20"))
                    .WithName("Yearly subscription 2");

        context.SubscriptionPlans.Add(subscriptionPlan4);

        var subscription = new Subscription()
        {
            SubscriptionPlan = subscriptionPlan,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddMonths(12),
            Status = SubscriptionStatus.Active,
            StatusDate = DateTime.Now
        };

        context.Subscriptions.Add(subscription);
    }
}