using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Enums;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public static class SubscriptionPlanFactory
{
    public static SubscriptionPlan CreateDailyPlan(int everyDays, TimeOnly startTime, TimeSpan? duration)
    {
        return new SubscriptionPlan
        {
            Schedule = SubscriptionSchedule.Daily(everyDays)
                .WithStartTime(startTime)
                .WithDuration(duration)
        };
    }

    public static SubscriptionPlan CreateWeeklyPlan(int everyWeeks, WeekDays onWeekDays, TimeOnly startTime, TimeSpan? duration)
    {
        var schedule = SubscriptionSchedule.Weekly(everyWeeks, onWeekDays)
                .WithStartTime(startTime)
                .WithDuration(duration);

        return new SubscriptionPlan()
            .WithSchedule(schedule);
    }

    public static SubscriptionPlan CreateQuarterlyPlan(int onDay, TimeOnly startTime, TimeSpan? duration)
    {
        return new SubscriptionPlan
        {
            Schedule = SubscriptionSchedule.Quarterly(onDay)
                .WithStartTime(startTime)
                .WithDuration(duration)
        };
    }

    public static SubscriptionPlan CreateMonthlyPlan(int everyMonths, int onDay, TimeOnly startTime, TimeSpan? duration)
    {
        return new SubscriptionPlan
        {
            Schedule = SubscriptionSchedule.Monthly(everyMonths, onDay)
                .WithStartTime(startTime)
                .WithDuration(duration)
        };
    }

    public static SubscriptionPlan CreateMonthlyPlan(int everyMonths, int onDay, DayOfWeek onDayOfWeek, TimeOnly startTime, TimeSpan? duration)
    {
        return new SubscriptionPlan
        {
            Schedule = SubscriptionSchedule.Monthly(everyMonths, onDay, onDayOfWeek)
                .WithStartTime(startTime)
                .WithDuration(duration)
        };
    }

    public static SubscriptionPlan CreateYearlyPlan(int everyYears, Month inMonth, int onDay, TimeOnly startTime, TimeSpan? duration)
    {
        return new SubscriptionPlan
        {
            Schedule = SubscriptionSchedule.Yearly(everyYears, inMonth, onDay)
                .WithStartTime(startTime)
                .WithDuration(duration)
        };
    }

    public static SubscriptionPlan CreateYearlyPlan(int everyYears, Month inMonth, int onDay, DayOfWeek onDayOfWeek, TimeOnly startTime, TimeSpan? duration)
    {
        return new SubscriptionPlan
        {
            Schedule = SubscriptionSchedule.Yearly(everyYears, inMonth, onDay, onDayOfWeek)
                .WithStartTime(startTime)
                .WithDuration(duration)
        };
    }
}