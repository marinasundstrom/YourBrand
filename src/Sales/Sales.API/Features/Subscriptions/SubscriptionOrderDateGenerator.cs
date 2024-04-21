using Dates.Recurring;
using Dates.Recurring.Builders;
using Dates.Recurring.Type;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Domain.Enums;

namespace YourBrand.Sales.Features.Subscriptions;

public class SubscriptionOrderDateGenerator
{
    /// <remarks>
    /// Make sure to pass dates without time.
    /// </remarks>
    public IEnumerable<(DateTime Start, DateTime? End)> GetOrderDatesFromSubscription(Subscription subscription, DateOnly? startDate = null, DateOnly? endDate = null)
    {
        DateTime? after = subscription.StartDate.ToDateTime(TimeOnly.MinValue);

        (DateTime Start, DateTime? End)? current = null;

        while (true)
        {
            current = GetOrderDate(subscription, after.GetValueOrDefault());
            after = current?.Start;

            if (current is null)
            {
                break;
            }

            yield return current.GetValueOrDefault();
        }
    }

    /// <remarks>
    /// Make sure to pass date without time.
    /// </remarks>
    public (DateTime Start, DateTime? End)? GetOrderDate(Subscription subscription, DateTime? after)
    {
        IRecurring? recurring = null;

        var subscriptionPlan = subscription.SubscriptionPlan;

        if (subscriptionPlan is null)
        {
            throw new NullReferenceException("SubscriptionPlan is null");
        }

        //var startDate = subscription.StartDate.Date;
        //var endDate = subscription.EndDate?.Date;

        if (subscription.StartDate >= subscription.EndDate)
        {
            throw new Exception("EndDate cannot occur before StartDate.");
        }

        StartingBuilder? startingBuilder = Recurs.Starting(subscription.StartDate.ToDateTime(TimeOnly.MinValue));

        Console.WriteLine(subscriptionPlan!.GetDescription());

        if (subscriptionPlan.Recurrence == Recurrence.Daily)
        {
            var dailyBuilder = startingBuilder
                .Every(subscriptionPlan.EveryDays.GetValueOrDefault())
                .Days();

            if (subscription.EndDate is not null)
            {
                dailyBuilder = dailyBuilder.Ending(subscription.EndDate.GetValueOrDefault().ToDateTime(TimeOnly.MinValue));
            }

            recurring = dailyBuilder.Build();
        }
        else if (subscriptionPlan.Recurrence == Recurrence.Weekly)
        {
            var weeklyBuilder = startingBuilder
                 .Every(subscriptionPlan.EveryWeeks.GetValueOrDefault())
                 .Weeks()
                 .OnDays(MapWeekDaysToDays(subscriptionPlan.OnWeekDays.GetValueOrDefault()));

            if (subscription.EndDate is not null)
            {
                weeklyBuilder = weeklyBuilder.Ending(subscription.EndDate.GetValueOrDefault().ToDateTime(TimeOnly.MinValue));
            }

            recurring = weeklyBuilder.Build();
        }
        else if (subscriptionPlan.Recurrence == Recurrence.Monthly)
        {
            var monthsBuilder = startingBuilder
                .Every(subscriptionPlan.EveryMonths.GetValueOrDefault())
                .Months();

            if (subscriptionPlan.OnDay is not null)
            {
                if (subscriptionPlan.OnDayOfWeek is null)
                {
                    monthsBuilder = monthsBuilder.OnDay(subscriptionPlan.OnDay.GetValueOrDefault());
                }
                else
                {
                    monthsBuilder = monthsBuilder.OnOrdinalWeek((Ordinal)subscriptionPlan.OnDay.GetValueOrDefault());
                    monthsBuilder = monthsBuilder.OnDay(subscriptionPlan.OnDayOfWeek.GetValueOrDefault());
                }
            }
            else
            {
                throw new Exception($"SubscriptionPlan {subscriptionPlan.Id} lacks OnDay.");
            }

            if (subscription.EndDate is not null)
            {
                monthsBuilder = monthsBuilder.Ending(subscription.EndDate.GetValueOrDefault().ToDateTime(TimeOnly.MinValue));
            }

            recurring = monthsBuilder.Build();
        }
        else if (subscriptionPlan.Recurrence == Recurrence.Yearly)
        {
            var yearsBuilder = startingBuilder
             .Every(subscriptionPlan.EveryYears.GetValueOrDefault())
             .Years();

            if (subscriptionPlan.OnDay is not null)
            {
                if (subscriptionPlan.InMonth is not null)
                {
                    if (subscriptionPlan.OnDayOfWeek is null)
                    {
                        yearsBuilder = yearsBuilder
                            .OnMonths((Dates.Recurring.Month)subscriptionPlan.InMonth.GetValueOrDefault())
                            .OnDay(subscriptionPlan.OnDay.GetValueOrDefault());
                    }
                    else
                    {
                        yearsBuilder = yearsBuilder
                            .OnMonths((Dates.Recurring.Month)subscriptionPlan.InMonth.GetValueOrDefault())
                            .OnOrdinalWeek((Ordinal)subscriptionPlan.OnDay.GetValueOrDefault())
                            .OnDay(subscriptionPlan.OnDayOfWeek.GetValueOrDefault());
                    }
                }
                else
                {
                    throw new Exception($"SubscriptionPlan {subscriptionPlan.Id} lacks InMonth.");
                }
            }
            else
            {
                throw new Exception($"SubscriptionPlan {subscriptionPlan.Id} lacks OnDay.");
            }

            if (subscription.EndDate is not null)
            {
                yearsBuilder = yearsBuilder.Ending(subscription.EndDate.GetValueOrDefault().ToDateTime(TimeOnly.MinValue));
            }

            recurring = yearsBuilder.Build();
        }

        if (recurring is null)
            throw new Exception();

        after = recurring.Next(after.GetValueOrDefault());

        if (after is not null)
        {
            var startDateTime = after.GetValueOrDefault().Add(subscriptionPlan.StartTime.GetValueOrDefault().ToTimeSpan());

            DateTime? endDateTime = null;

            if (subscriptionPlan.Duration is not null)
            {
                // Duration might be Null (Unknown)

                endDateTime = startDateTime.Add(subscriptionPlan.Duration.GetValueOrDefault());
            }

            return (
                startDateTime,
                endDateTime);
        }

        return null;
    }

    private static Day MapWeekDaysToDays(WeekDays weekDays)
    {
        Day day = 0;

        if (weekDays.HasFlag(WeekDays.Monday))
        {
            day |= Day.MONDAY;
        }
        if (weekDays.HasFlag(WeekDays.Tuesday))
        {
            day |= Day.TUESDAY;
        }
        if (weekDays.HasFlag(WeekDays.Wednesday))
        {
            day |= Day.WEDNESDAY;
        }
        if (weekDays.HasFlag(WeekDays.Thursday))
        {
            day |= Day.THURSDAY;
        }
        if (weekDays.HasFlag(WeekDays.Friday))
        {
            day |= Day.FRIDAY;
        }
        if (weekDays.HasFlag(WeekDays.Saturday))
        {
            day |= Day.SATURDAY;
        }

        return day;
    }
}