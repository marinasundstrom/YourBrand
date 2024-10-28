using System;

using Humanizer;

namespace YourBrand.Sales;

public static class SubscriptionPlanExtensions
{
    public static string GetDescription(this SubscriptionPlan subscriptionPlan)
    {
        var schedule = subscriptionPlan.Schedule;

        if (schedule.Frequency == TimeInterval.Daily)
        {
            return $"Every {(schedule.EveryDays == 1 ? string.Empty : schedule.EveryDays.GetValueOrDefault().Ordinalize())} day";
        }
        else if (schedule.Frequency == TimeInterval.Weekly)
        {
            return $"Every {(schedule.EveryWeeks == 1 ? string.Empty : schedule.EveryWeeks.GetValueOrDefault().Ordinalize())} week on {schedule.OnWeekDays.GetValueOrDefault()}";
        }
        else if (schedule.Frequency == TimeInterval.Monthly)
        {
            if (schedule.OnDayOfWeek is null)
            {
                return $"Every {(schedule.EveryMonths == 1 ? string.Empty : schedule.EveryMonths.GetValueOrDefault().Ordinalize())} month on the {schedule.OnDay.GetValueOrDefault().Ordinalize()} day";
            }
            else
            {
                return $"Every {(schedule.EveryMonths == 1 ? string.Empty : schedule.EveryMonths.GetValueOrDefault().Ordinalize())} month on the {schedule.OnDay.GetValueOrDefault().Ordinalize()} {schedule.OnDayOfWeek}";
            }
        }
        else if (schedule.Frequency == TimeInterval.Yearly)
        {
            if (schedule.OnDayOfWeek is null)
            {
                return $"Every {(schedule.EveryYears == 1 ? string.Empty : schedule.EveryYears.GetValueOrDefault().Ordinalize())} year on {schedule.InMonth} {schedule.OnDay.GetValueOrDefault().Ordinalize()}";
            }
            else
            {
                return $"Every {(schedule.EveryYears == 1 ? string.Empty : schedule.EveryYears.GetValueOrDefault().Ordinalize())} year on the {schedule.OnDay.GetValueOrDefault().Ordinalize()} {schedule.OnDayOfWeek} in {schedule.InMonth}";
            }
        }

        throw new Exception();
    }

    public static string GetDescription(this ISubscriptionParameters schedule)
    {
        if (schedule.Frequency == TimeInterval.Daily)
        {
            return $"Every {(schedule.EveryDays == 1 ? string.Empty : schedule.EveryDays.GetValueOrDefault().Ordinalize())} day";
        }
        else if (schedule.Frequency == TimeInterval.Weekly)
        {
            return $"Every {(schedule.EveryWeeks == 1 ? string.Empty : schedule.EveryWeeks.GetValueOrDefault().Ordinalize())} week on {schedule.OnWeekDays.GetValueOrDefault()}";
        }
        else if (schedule.Frequency == TimeInterval.Monthly)
        {
            if (schedule.OnDayOfWeek is null)
            {
                return $"Every {(schedule.EveryMonths == 1 ? string.Empty : schedule.EveryMonths.GetValueOrDefault().Ordinalize())} month on the {schedule.OnDay.GetValueOrDefault().Ordinalize()} day";
            }
            else
            {
                return $"Every {(schedule.EveryMonths == 1 ? string.Empty : schedule.EveryMonths.GetValueOrDefault().Ordinalize())} month on the {schedule.OnDay.GetValueOrDefault().Ordinalize()} {schedule.OnDayOfWeek}";
            }
        }
        else if (schedule.Frequency == TimeInterval.Yearly)
        {
            if (schedule.OnDayOfWeek is null)
            {
                return $"Every {(schedule.EveryYears == 1 ? string.Empty : schedule.EveryYears.GetValueOrDefault().Ordinalize())} year on {schedule.InMonth} {schedule.OnDay.GetValueOrDefault().Ordinalize()}";
            }
            else
            {
                return $"Every {(schedule.EveryYears == 1 ? string.Empty : schedule.EveryYears.GetValueOrDefault().Ordinalize())} year on the {schedule.OnDay.GetValueOrDefault().Ordinalize()} {schedule.OnDayOfWeek} in {schedule.InMonth}";
            }
        }

        throw new Exception();
    }
}