using System;

using YourBrand.Sales.Domain.Entities;

namespace YourBrand.Sales.Features.Subscriptions;

static class SubscriptionPlanDumper
{
    public static void Dump(this SubscriptionPlan subscriptionPlan)
    {
        Console.WriteLine(subscriptionPlan.GetDescription());
    }
}