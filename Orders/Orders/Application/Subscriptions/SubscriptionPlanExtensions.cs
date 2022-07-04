using System;

using Humanizer;

using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Domain.Enums;

namespace YourBrand.Orders.Application.Subscriptions
{
    public static class SubscriptionPlanExtensions
    {
        public static string GetDescription(this SubscriptionPlan subscriptionPlan)
        {
            if (subscriptionPlan.Recurrence == Recurrence.Daily)
            {
                return $"Every {subscriptionPlan.EveryDays.GetValueOrDefault()} {(subscriptionPlan.EveryDays == 1 ? "day" : "day".Pluralize())}";
            }
            else if (subscriptionPlan.Recurrence == Recurrence.Weekly)
            {
                return $"Every {subscriptionPlan.EveryWeeks.GetValueOrDefault()} {(subscriptionPlan.EveryWeeks == 1 ? "week" : "week".Pluralize())} on {subscriptionPlan.OnWeekDays.GetValueOrDefault()}";
            }
            else if (subscriptionPlan.Recurrence == Recurrence.Monthly)
            {
                if (subscriptionPlan.OnDayOfWeek is null)
                {
                    return $"Every {subscriptionPlan.EveryMonths.GetValueOrDefault()} {(subscriptionPlan.EveryMonths == 1 ? "month" : "month".Pluralize())} on the {subscriptionPlan.OnDay.GetValueOrDefault().Ordinalize()} day";
                }
                else
                {
                    return $"Every {subscriptionPlan.EveryMonths.GetValueOrDefault()} {(subscriptionPlan.EveryMonths == 1 ? "month" : "month".Pluralize())} on the {subscriptionPlan.OnDay.GetValueOrDefault().Ordinalize()} {subscriptionPlan.OnDayOfWeek}";
                }
            }
            else if (subscriptionPlan.Recurrence == Recurrence.Yearly)
            {
                if (subscriptionPlan.OnDayOfWeek is null)
                {
                    return $"Every {subscriptionPlan.EveryYears.GetValueOrDefault()} {(subscriptionPlan.EveryYears == 1 ? "year" : "year".Pluralize())} on {subscriptionPlan.InMonth} {subscriptionPlan.OnDay.GetValueOrDefault().Ordinalize()}";
                }
                else
                {
                    return $"Every {subscriptionPlan.EveryYears.GetValueOrDefault()} {(subscriptionPlan.EveryYears == 1 ? "year" : "year".Pluralize())} on the {subscriptionPlan.OnDay.GetValueOrDefault().Ordinalize()} {subscriptionPlan.OnDayOfWeek} in {subscriptionPlan.InMonth}";
                }
            }

            throw new Exception();
        }
    }
}