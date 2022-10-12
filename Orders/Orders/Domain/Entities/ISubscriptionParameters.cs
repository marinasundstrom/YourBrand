using YourBrand.Orders.Domain.Enums;

namespace YourBrand.Orders.Domain.Entities
{
    public interface ISubscriptionParameters
    {
        bool AutoRenew { get; set; }
        Recurrence Recurrence { get; set; }
        int? EveryDays { get; set; }
        int? EveryWeeks { get; set; }
        WeekDays? OnWeekDays { get; set; }
        int? EveryMonths { get; set; }
        int? EveryYears { get; set; }
        int? OnDay { get; set; }
        DayOfWeek? OnDayOfWeek { get; set; }
        Month? InMonth { get; set; }
        TimeSpan StartTime { get; set; }
        TimeSpan? Duration { get; set; }
    }
}