using Dates.Recurring;
using Dates.Recurring.Builders;
using Dates.Recurring.Type;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Enums;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public class SubscriptionOrderDateGenerator
{
    /// <remarks>
    /// Make sure to pass dates without time.
    /// </remarks>
    public IEnumerable<(DateTime Start, DateTime? End)> GetOrderDatesFromSubscription(Subscription subscription, DateOnly? startDate = null, DateOnly? endDate = null)
    {
        DateTime after = subscription.StartDate.ToDateTime(TimeOnly.MinValue);
        var generator = CreateGeneratorFromSubscription(subscription);

        while (true)
        {
            var current = GetOrderNextDate(generator, subscription.Plan!, after);
            if (current is null) break;

            yield return current.Value;
            after = current.Value.Start;
        }
    }

    public IRecurring CreateGeneratorFromSubscription(Subscription subscription)
    {
        if (subscription.Plan is null)
            throw new NullReferenceException("SubscriptionPlan is null");

        if (subscription.StartDate >= subscription.EndDate)
            throw new Exception("EndDate cannot occur before StartDate.");

        var plan = subscription.Plan;
        var schedule = plan.Schedule;

        var startDateTime = subscription.StartDate.ToDateTime(TimeOnly.MinValue);
        var startingBuilder = Recurs.Starting(startDateTime);

        return schedule.Frequency switch
        {
            TimeInterval.Daily => BuildDailyRecurring(startingBuilder, plan, subscription.EndDate),
            TimeInterval.Weekly => BuildWeeklyRecurring(startingBuilder, plan, subscription.EndDate),
            TimeInterval.Monthly => BuildMonthlyRecurring(startingBuilder, plan, subscription.EndDate),
            TimeInterval.Quarterly => BuildQuarterlyRecurring(startingBuilder, plan, subscription.EndDate),
            TimeInterval.Yearly => BuildYearlyRecurring(startingBuilder, plan, subscription.EndDate),
            _ => throw new NotSupportedException($"Recurrence type {schedule.Frequency} is not supported.")
        };
    }

    private IRecurring BuildDailyRecurring(StartingBuilder builder, SubscriptionPlan plan, DateOnly? endDate)
    {
        var schedule = plan.Schedule;

        var dailyBuilder = builder.Every(schedule.EveryDays ?? 1).Days();
        if (endDate != null)
        {
            dailyBuilder = dailyBuilder.Ending(endDate.Value.ToDateTime(TimeOnly.MinValue));
        }
        return dailyBuilder.Build();
    }

    private IRecurring BuildWeeklyRecurring(StartingBuilder builder, SubscriptionPlan plan, DateOnly? endDate)
    {
        var schedule = plan.Schedule;

        var weeklyBuilder = builder
            .Every(schedule.EveryWeeks ?? 1)
            .Weeks()
            .OnDays(MapWeekDaysToDays(schedule.OnWeekDays ?? WeekDays.Monday));

        if (endDate != null)
        {
            weeklyBuilder = weeklyBuilder.Ending(endDate.Value.ToDateTime(TimeOnly.MinValue));
        }

        return weeklyBuilder.Build();
    }

    private IRecurring BuildMonthlyRecurring(StartingBuilder builder, SubscriptionPlan plan, DateOnly? endDate)
    {
        var schedule = plan.Schedule;

        if (schedule.OnDay is null)
            throw new Exception($"SubscriptionPlan {plan.Id} lacks OnDay.");

        var monthsBuilder = builder.Every(schedule.EveryMonths ?? 1).Months();

        monthsBuilder = schedule.OnDayOfWeek is null
            ? monthsBuilder.OnDay(schedule.OnDay.Value)
            : monthsBuilder.OnOrdinalWeek((Ordinal)schedule.OnDay.Value).OnDay(schedule.OnDayOfWeek.Value);

        if (endDate != null)
        {
            monthsBuilder = monthsBuilder.Ending(endDate.Value.ToDateTime(TimeOnly.MinValue));
        }

        return monthsBuilder.Build();
    }

    private IRecurring BuildQuarterlyRecurring(StartingBuilder builder, SubscriptionPlan plan, DateOnly? endDate)
    {
        var schedule = plan.Schedule;

        if (schedule.OnDay is null)
            throw new Exception($"SubscriptionPlan {plan.Id} lacks OnDay.");

        var quarterlyBuilder = builder.Every(3).Months();

        quarterlyBuilder = schedule.OnDayOfWeek is null
            ? quarterlyBuilder.OnDay(schedule.OnDay.Value)
            : quarterlyBuilder.OnOrdinalWeek((Ordinal)schedule.OnDay.Value).OnDay(schedule.OnDayOfWeek.Value);

        if (endDate != null)
        {
            quarterlyBuilder = quarterlyBuilder.Ending(endDate.Value.ToDateTime(TimeOnly.MinValue));
        }

        return quarterlyBuilder.Build();
    }

    private IRecurring BuildYearlyRecurring(StartingBuilder builder, SubscriptionPlan plan, DateOnly? endDate)
    {
        var schedule = plan.Schedule;

        if (schedule.OnDay is null || schedule.InMonth is null)
            throw new Exception($"SubscriptionPlan {plan.Id} lacks OnDay or InMonth.");

        var yearsBuilder = builder
            .Every(schedule.EveryYears ?? 1)
            .Years()
            .OnMonths((Dates.Recurring.Month)schedule.InMonth.Value);

        yearsBuilder = schedule.OnDayOfWeek is null
            ? yearsBuilder.OnDay(schedule.OnDay.Value)
            : yearsBuilder.OnOrdinalWeek((Ordinal)schedule.OnDay.Value).OnDay(schedule.OnDayOfWeek.Value);

        if (endDate != null)
        {
            yearsBuilder = yearsBuilder.Ending(endDate.Value.ToDateTime(TimeOnly.MinValue));
        }

        return yearsBuilder.Build();
    }

    public (DateTime Start, DateTime? End)? GetOrderNextDate(IRecurring recurring, SubscriptionPlan plan, DateTime after)
    {
        var nextDate = recurring.Next(after);
        if (nextDate is null) return null;

        var schedule = plan.Schedule;

        var startDateTime = nextDate.Value.Add(schedule.StartTime?.ToTimeSpan() ?? TimeSpan.Zero);
        var endDateTime = schedule.Duration is not null
            ? startDateTime.Add(schedule.Duration.Value)
            : (DateTime?)null;

        return (startDateTime, endDateTime);
    }

    private static Day MapWeekDaysToDays(WeekDays weekDays)
    {
        return (Day)weekDays;
    }
}