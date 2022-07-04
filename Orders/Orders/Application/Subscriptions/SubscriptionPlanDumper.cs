using System;

using YourBrand.Orders.Domain.Entities;

namespace YourBrand.Orders.Application.Subscriptions
{
    static class SubscriptionPlanDumper
    {
        public static void Dump(this SubscriptionPlan subscriptionPlan)
        {
            Console.WriteLine(subscriptionPlan.GetDescription());
        }
    }
}