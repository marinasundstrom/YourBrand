using YourBrand.Sales.Domain.Enums;

namespace YourBrand.Sales.Domain.Entities;

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
    TimeOnly? StartTime { get; set; }
    TimeSpan? Duration { get; set; }
}