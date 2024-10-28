using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Enums;
using YourBrand.Sales.Features.SubscriptionManagement;

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
        context.SubscriptionTypes.Add(new SubscriptionType(1, "Recurring delivery", "recurring-delivery", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.SubscriptionTypes.Add(new SubscriptionType(2, "Access subscription", "access-subscription", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });

        context.SubscriptionStatuses.Add(new SubscriptionStatus(1, "Pending", "pending", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.SubscriptionStatuses.Add(new SubscriptionStatus(2, "Active", "active", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.SubscriptionStatuses.Add(new SubscriptionStatus(3, "Trial", "trial", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.SubscriptionStatuses.Add(new SubscriptionStatus(4, "Paused", "paused", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.SubscriptionStatuses.Add(new SubscriptionStatus(5, "Canceled", "canceled", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.SubscriptionStatuses.Add(new SubscriptionStatus(6, "Expired", "expired", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.SubscriptionStatuses.Add(new SubscriptionStatus(7, "Suspended", "suspended", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });

        context.OrderTypes.Add(new OrderType(1, "Order", "order", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderTypes.Add(new OrderType(2, "Subscription", "subscription", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderTypes.Add(new OrderType(3, "Delivery order", "delivery-order", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderTypes.Add(new OrderType(4, "Subscription renewal", "subscription-renewal", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });

        context.OrderStatuses.Add(new OrderStatus(1, "Draft", "draft", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(2, "Planned", "planned", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(3, "Pending Confirmation", "pending-confirmation", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(4, "Confirmed", "confirmed", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(5, "Payment processing", "payment-processing", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(6, "Payment failed", "payment-failed", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(7, "Processing", "processing", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(8, "Shipped", "shipped", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(9, "In transit", "in-transit", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(10, "Out for delivery", "out-for-delivery", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(11, "Delivered", "delivered", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(12, "Completed", "completed", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(13, "Canceled", "canceled", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(14, "On hold", "on-hold", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId,
        });
        context.OrderStatuses.Add(new OrderStatus(15, "Returned", "returned", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });
        context.OrderStatuses.Add(new OrderStatus(16, "Refunded", "refunded", string.Empty)
        {
            OrganizationId = TenantConstants.OrganizationId
        });

        var subscriptionPlan0 = SubscriptionPlan.Create(SubscriptionPlanType.RecurringOrder, "Bi-weekly subscription")
            .WithSchedule(SubscriptionSchedule.Weekly(1, WeekDays.Tuesday | WeekDays.Thursday)
                .WithStartTime(TimeOnly.Parse("16:00"))
                .WithEndTime(TimeOnly.Parse("17:00")));

        context.SubscriptionPlans.Add(subscriptionPlan0);

        var subscriptionPlan1 = SubscriptionPlan.Create(SubscriptionPlanType.RecurringOrder, "Monthly subscription 1")
            .WithSchedule(SubscriptionSchedule.Monthly(1, 1, DayOfWeek.Tuesday)
                .WithStartTime(TimeOnly.Parse("10:30"))
                .WithDuration(TimeSpan.Parse("00:30")));

        context.SubscriptionPlans.Add(subscriptionPlan1);

        var subscriptionPlan2 = SubscriptionPlan.Create(SubscriptionPlanType.RecurringOrder, "Monthly subscription 2")
            .WithSchedule(SubscriptionSchedule.Monthly(1, 1, DayOfWeek.Monday | DayOfWeek.Friday)
                .WithStartTime(TimeOnly.Parse("07:30")));

        context.SubscriptionPlans.Add(subscriptionPlan2);

        var subscriptionPlan3 = SubscriptionPlan.Create(SubscriptionPlanType.RecurringOrder, "Yearly subscription 1")
            .WithSchedule(SubscriptionSchedule.Yearly(1, Month.April, 15)
                .WithStartTime(TimeOnly.Parse("14:30"))
                .WithDuration(TimeSpan.Parse("01:00")));

        context.SubscriptionPlans.Add(subscriptionPlan3);

        var subscriptionPlan4 = SubscriptionPlan.Create(SubscriptionPlanType.RecurringOrder, "Yearly subscription 2")
            .WithSchedule(SubscriptionSchedule.Yearly(1, Month.July, 3, DayOfWeek.Thursday)
                .WithStartTime(TimeOnly.Parse("09:00"))
                .WithDuration(TimeSpan.Parse("00:20")));

        context.SubscriptionPlans.Add(subscriptionPlan4);

        var subscriptionPlan6 = SubscriptionPlan.Create(SubscriptionPlanType.RecurringOrder, "Monthly subscription with 90 days trial and 30 days cancellation finalization")
            .WithSchedule(SubscriptionSchedule.Monthly(1, 4, DayOfWeek.Tuesday)
                .WithStartTime(TimeOnly.Parse("10:30"))
                .WithDuration(TimeSpan.FromMinutes(30)))
                .WithTrial(90)
                .WithCancellationFinalizationPeriod(TimeSpan.FromDays(30));

        context.SubscriptionPlans.Add(subscriptionPlan6);
    }
}