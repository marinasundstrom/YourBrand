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
            var current = GetOrderNextDate(generator, subscription.SubscriptionPlan!, after);
            if (current is null) break;

            yield return current.Value;
            after = current.Value.Start;
        }
    }

    public IRecurring CreateGeneratorFromSubscription(Subscription subscription)
    {
        if (subscription.SubscriptionPlan is null)
            throw new NullReferenceException("SubscriptionPlan is null");

        if (subscription.StartDate >= subscription.EndDate)
            throw new Exception("EndDate cannot occur before StartDate.");

        var plan = subscription.SubscriptionPlan;
        var startDateTime = subscription.StartDate.ToDateTime(TimeOnly.MinValue);
        var startingBuilder = Recurs.Starting(startDateTime);

        return plan.Recurrence switch
        {
            Recurrence.Daily => BuildDailyRecurring(startingBuilder, plan, subscription.EndDate),
            Recurrence.Weekly => BuildWeeklyRecurring(startingBuilder, plan, subscription.EndDate),
            Recurrence.Monthly => BuildMonthlyRecurring(startingBuilder, plan, subscription.EndDate),
            Recurrence.Quarterly => BuildQuarterlyRecurring(startingBuilder, plan, subscription.EndDate),
            Recurrence.Yearly => BuildYearlyRecurring(startingBuilder, plan, subscription.EndDate),
            _ => throw new NotSupportedException($"Recurrence type {plan.Recurrence} is not supported.")
        };
    }

    private IRecurring BuildDailyRecurring(StartingBuilder builder, SubscriptionPlan plan, DateOnly? endDate)
    {
        var dailyBuilder = builder.Every(plan.EveryDays ?? 1).Days();
        if (endDate != null)
        {
            dailyBuilder = dailyBuilder.Ending(endDate.Value.ToDateTime(TimeOnly.MinValue));
        }
        return dailyBuilder.Build();
    }

    private IRecurring BuildWeeklyRecurring(StartingBuilder builder, SubscriptionPlan plan, DateOnly? endDate)
    {
        var weeklyBuilder = builder
            .Every(plan.EveryWeeks ?? 1)
            .Weeks()
            .OnDays(MapWeekDaysToDays(plan.OnWeekDays ?? WeekDays.Monday));

        if (endDate != null)
        {
            weeklyBuilder = weeklyBuilder.Ending(endDate.Value.ToDateTime(TimeOnly.MinValue));
        }

        return weeklyBuilder.Build();
    }

    private IRecurring BuildMonthlyRecurring(StartingBuilder builder, SubscriptionPlan plan, DateOnly? endDate)
    {
        if (plan.OnDay is null)
            throw new Exception($"SubscriptionPlan {plan.Id} lacks OnDay.");

        var monthsBuilder = builder.Every(plan.EveryMonths ?? 1).Months();

        monthsBuilder = plan.OnDayOfWeek is null
            ? monthsBuilder.OnDay(plan.OnDay.Value)
            : monthsBuilder.OnOrdinalWeek((Ordinal)plan.OnDay.Value).OnDay(plan.OnDayOfWeek.Value);

        if (endDate != null)
        {
            monthsBuilder = monthsBuilder.Ending(endDate.Value.ToDateTime(TimeOnly.MinValue));
        }

        return monthsBuilder.Build();
    }

    private IRecurring BuildQuarterlyRecurring(StartingBuilder builder, SubscriptionPlan plan, DateOnly? endDate)
    {
        if (plan.OnDay is null)
            throw new Exception($"SubscriptionPlan {plan.Id} lacks OnDay.");

        var quarterlyBuilder = builder.Every(3).Months();

        quarterlyBuilder = plan.OnDayOfWeek is null
            ? quarterlyBuilder.OnDay(plan.OnDay.Value)
            : quarterlyBuilder.OnOrdinalWeek((Ordinal)plan.OnDay.Value).OnDay(plan.OnDayOfWeek.Value);

        if (endDate != null)
        {
            quarterlyBuilder = quarterlyBuilder.Ending(endDate.Value.ToDateTime(TimeOnly.MinValue));
        }

        return quarterlyBuilder.Build();
    }

    private IRecurring BuildYearlyRecurring(StartingBuilder builder, SubscriptionPlan plan, DateOnly? endDate)
    {
        if (plan.OnDay is null || plan.InMonth is null)
            throw new Exception($"SubscriptionPlan {plan.Id} lacks OnDay or InMonth.");

        var yearsBuilder = builder
            .Every(plan.EveryYears ?? 1)
            .Years()
            .OnMonths((Dates.Recurring.Month)plan.InMonth.Value);

        yearsBuilder = plan.OnDayOfWeek is null
            ? yearsBuilder.OnDay(plan.OnDay.Value)
            : yearsBuilder.OnOrdinalWeek((Ordinal)plan.OnDay.Value).OnDay(plan.OnDayOfWeek.Value);

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

        var startDateTime = nextDate.Value.Add(plan.StartTime?.ToTimeSpan() ?? TimeSpan.Zero);
        var endDateTime = plan.Duration is not null
            ? startDateTime.Add(plan.Duration.Value)
            : (DateTime?)null;

        return (startDateTime, endDateTime);
    }

    private static Day MapWeekDaysToDays(WeekDays weekDays)
    {
        return (Day)weekDays;
    }
}
