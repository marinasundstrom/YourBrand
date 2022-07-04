using System;

using YourBrand.Orders.Domain.Entities;
using YourBrand.Orders.Domain.Enums;

namespace YourBrand.Orders.Application.Subscriptions
{
    public static class SubscriptionPlanFactory
    {
        public static SubscriptionPlan CreateDailyPlan(int everyDays, TimeSpan startTime, TimeSpan? duration)
        {
            return new SubscriptionPlan
            {
                Recurrence = Recurrence.Daily,
                EveryDays = everyDays,
                StartTime = startTime,
                Duration = duration
            };
        }

        public static SubscriptionPlan CreateWeeklyPlan(int everyWeeks, WeekDays onWeekDays, TimeSpan startTime, TimeSpan? duration)
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

        public static SubscriptionPlan CreateMonthlyPlan(int everyMonths, int onDay, TimeSpan startTime, TimeSpan? duration)
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

        public static SubscriptionPlan CreateMonthlyPlan(int everyMonths, int onDay, DayOfWeek onDayOfWeek, TimeSpan startTime, TimeSpan? duration)
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

        public static SubscriptionPlan CreateYearlyPlan(int everyYears, Month inMonth, int onDay, TimeSpan startTime, TimeSpan? duration)
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

        public static SubscriptionPlan CreateYearlyPlan(int everyYears, Month inMonth, int onDay, DayOfWeek onDayOfWeek, TimeSpan startTime, TimeSpan? duration)
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
}