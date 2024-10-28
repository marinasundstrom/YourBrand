using YourBrand.Sales.Domain.Enums;

namespace YourBrand.Sales.Domain.Entities;

public static class SubscriptionScheduleExtensions
{
    public static DateTimeOffset GetNextDate(this SubscriptionSchedule schedule, DateTimeOffset fromDate)
    {
        return schedule.Frequency switch
        {
            TimeInterval.Daily => fromDate.AddDays(schedule.EveryDays ?? 1),
            TimeInterval.Weekly => fromDate.AddDays((schedule.EveryWeeks ?? 1) * 7),
            TimeInterval.Monthly => fromDate.AddMonths(schedule.EveryMonths ?? 1),
            TimeInterval.Quarterly => fromDate.AddMonths(3),
            TimeInterval.Yearly => fromDate.AddYears(schedule.EveryYears ?? 1),
            _ => throw new InvalidOperationException("Unsupported frequency."),
        };
    }
}