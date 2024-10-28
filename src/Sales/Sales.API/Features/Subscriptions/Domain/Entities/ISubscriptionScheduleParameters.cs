using YourBrand.Sales.Domain.Enums;

namespace YourBrand.Sales.Domain.Entities;

public interface ISubscriptionScheduleParameters
{
    TimeInterval Frequency { get; set; }
    int? EveryDays { get; set; }
    int? EveryWeeks { get; set; }
    WeekDays? OnWeekDays { get; set; }
    int? EveryMonths { get; set; } // Every two months
    int? EveryYears { get; set; }
    int? OnDay { get; set; } // 3rd of January. If OnDayOfWeek set to i.e. Tuesday, 3rd Tuesday
    DayOfWeek? OnDayOfWeek { get; set; }
    Month? InMonth { get; set; }
    TimeOnly? StartTime { get; set; }
    TimeSpan? Duration { get; set; }
}
