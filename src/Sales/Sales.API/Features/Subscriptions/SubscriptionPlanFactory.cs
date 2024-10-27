using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Enums;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public static class SubscriptionPlanFactory
{
    public static SubscriptionPlan CreateDailyPlan(int everyDays, TimeOnly startTime, TimeSpan? duration)
    {
        return new SubscriptionPlan
        {
            Recurrence = Recurrence.Daily,
            EveryDays = everyDays,
            StartTime = startTime,
            Duration = duration
        };
    }

    public static SubscriptionPlan CreateWeeklyPlan(int everyWeeks, WeekDays onWeekDays, TimeOnly startTime, TimeSpan? duration)
    {
        return new SubscriptionPlan
        {
            Recurrence = Recurrence.Weekly,
            EveryWeeks = everyWeeks,
            OnWeekDays = onWeekDays,
            StartTime = startTime,
            Duration = duration
        };
    }

    public static SubscriptionPlan CreateQuarterlyPlan(int onDay, TimeOnly startTime, TimeSpan? duration)
    {
        return new SubscriptionPlan
        {
            Recurrence = Recurrence.Quarterly,
            OnDay = onDay,
            StartTime = startTime,
            Duration = duration
        };
    }

    public static SubscriptionPlan CreateMonthlyPlan(int everyMonths, int onDay, TimeOnly startTime, TimeSpan? duration)
    {
        return new SubscriptionPlan
        {
            Recurrence = Recurrence.Monthly,
            EveryMonths = everyMonths,
            OnDay = onDay,
            //OnDayOfWeek = DayOfWeek.Tuesday,
            StartTime = startTime,
            Duration = duration
        };
    }

    public static SubscriptionPlan CreateMonthlyPlan(int everyMonths, int onDay, DayOfWeek onDayOfWeek, TimeOnly startTime, TimeSpan? duration)
    {
        return new SubscriptionPlan
        {
            Recurrence = Recurrence.Monthly,
            EveryMonths = everyMonths,
            OnDay = onDay,
            OnDayOfWeek = onDayOfWeek,
            StartTime = startTime,
            Duration = duration
        };
    }

    public static SubscriptionPlan CreateYearlyPlan(int everyYears, Month inMonth, int onDay, TimeOnly startTime, TimeSpan? duration)
    {
        return new SubscriptionPlan
        {
            Recurrence = Recurrence.Yearly,
            EveryYears = everyYears,
            OnDay = onDay,
            InMonth = inMonth,
            StartTime = startTime,
            Duration = duration
        };
    }

    public static SubscriptionPlan CreateYearlyPlan(int everyYears, Month inMonth, int onDay, DayOfWeek onDayOfWeek, TimeOnly startTime, TimeSpan? duration)
    {
        return new SubscriptionPlan
        {
            Recurrence = Recurrence.Yearly,
            EveryYears = everyYears,
            OnDay = onDay,
            OnDayOfWeek = onDayOfWeek,
            InMonth = inMonth,
            StartTime = startTime,
            Duration = duration
        };
    }
}