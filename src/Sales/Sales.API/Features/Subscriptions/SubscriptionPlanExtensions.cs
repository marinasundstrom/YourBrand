using System;

using Humanizer;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Enums;

namespace YourBrand.Sales.Features.Subscriptions;

public static class SubscriptionPlanExtensions
{
    public static string GetDescription(this SubscriptionPlan subscriptionPlan)
    {
        if (subscriptionPlan.Recurrence == Recurrence.Daily)
        {
            return $"Every {(subscriptionPlan.EveryDays == 1 ? string.Empty : subscriptionPlan.EveryDays.GetValueOrDefault().Ordinalize())} day";
        }
        else if (subscriptionPlan.Recurrence == Recurrence.Weekly)
        {
            return $"Every {(subscriptionPlan.EveryWeeks == 1 ? string.Empty : subscriptionPlan.EveryWeeks.GetValueOrDefault().Ordinalize())} week on {subscriptionPlan.OnWeekDays.GetValueOrDefault()}";
        }
        else if (subscriptionPlan.Recurrence == Recurrence.Monthly)
        {
            if (subscriptionPlan.OnDayOfWeek is null)
            {
                return $"Every {(subscriptionPlan.EveryMonths == 1 ? string.Empty : subscriptionPlan.EveryMonths.GetValueOrDefault().Ordinalize())} month on the {subscriptionPlan.OnDay.GetValueOrDefault().Ordinalize()} day";
            }
            else
            {
                return $"Every {(subscriptionPlan.EveryMonths == 1 ? string.Empty : subscriptionPlan.EveryMonths.GetValueOrDefault().Ordinalize())} month on the {subscriptionPlan.OnDay.GetValueOrDefault().Ordinalize()} {subscriptionPlan.OnDayOfWeek}";
            }
        }
        else if (subscriptionPlan.Recurrence == Recurrence.Yearly)
        {
            if (subscriptionPlan.OnDayOfWeek is null)
            {
                return $"Every {(subscriptionPlan.EveryYears == 1 ? string.Empty : subscriptionPlan.EveryYears.GetValueOrDefault().Ordinalize())} year on {subscriptionPlan.InMonth} {subscriptionPlan.OnDay.GetValueOrDefault().Ordinalize()}";
            }
            else
            {
                return $"Every {(subscriptionPlan.EveryYears == 1 ? string.Empty : subscriptionPlan.EveryYears.GetValueOrDefault().Ordinalize())} year on the {subscriptionPlan.OnDay.GetValueOrDefault().Ordinalize()} {subscriptionPlan.OnDayOfWeek} in {subscriptionPlan.InMonth}";
            }
        }

        throw new Exception();
    }
}