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
        context.OrderStatuses.Add(new OrderStatus(1, "Draft", "draft", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(2, "Open", "open", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(3, "Archived", "archived", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(4, "Canceled", "canceled", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId,
        });

        var subscriptionPlan0 = SubscriptionPlanFactory
                    .CreateWeeklyPlan(1, WeekDays.Tuesday | WeekDays.Thursday, TimeOnly.Parse("16:00"), null)
                    .WithName("Bi-weekly subscription")
                    .WithEndTime(TimeOnly.Parse("17:00"));

        context.SubscriptionPlans.Add(subscriptionPlan0);

        var subscriptionPlan = SubscriptionPlanFactory
                   .CreateMonthlyPlan(1, 1, DayOfWeek.Tuesday, TimeOnly.Parse("10:30"), TimeSpan.Parse("00:30"))
                   .WithName("Monthly subscription 1");

        context.SubscriptionPlans.Add(subscriptionPlan);

        var subscriptionPlan2 = SubscriptionPlanFactory
            .CreateMonthlyPlan(1, 1, DayOfWeek.Tuesday, TimeOnly.Parse("07:30"), null) // , TimeSpan.Parse("00:45"))
            .WithName("Monthly subscription 2");

        context.SubscriptionPlans.Add(subscriptionPlan2);

        var subscriptionPlan3 = SubscriptionPlanFactory
                    .CreateYearlyPlan(1, Month.April, 15, TimeOnly.Parse("14:30"), TimeSpan.Parse("00:30"))
                    .WithName("Yearly subscription 1");

        context.SubscriptionPlans.Add(subscriptionPlan3);

        var subscriptionPlan4 = SubscriptionPlanFactory
                    .CreateYearlyPlan(1, Month.April, 3, DayOfWeek.Thursday, TimeOnly.Parse("09:00"), TimeSpan.Parse("00:20"))
                    .WithName("Yearly subscription 2");

        context.SubscriptionPlans.Add(subscriptionPlan4);

        var subscription = new Subscription()
        {
            SubscriptionPlan = subscriptionPlan,
            StartDate = DateOnly.FromDateTime(DateTime.Now),
            EndDate = DateOnly.FromDateTime(DateTime.Now).AddMonths(12),
            Status = SubscriptionStatus.Active,
            StatusDate = DateTime.Now
        };

        subscription.OrganizationId = TenantConstants.OrganizationId;

        context.Subscriptions.Add(subscription);
    }
}