namespace YourBrand.Sales;

public interface ISubscriptionParameters
{
    bool AutoRenew { get; }
    Recurrence Recurrence { get; }
    int? EveryDays { get; }
    int? EveryWeeks { get; }
    WeekDays? OnWeekDays { get; }
    int? EveryMonths { get; }
    int? EveryYears { get; }
    int? OnDay { get; }
    DayOfWeek? OnDayOfWeek { get; }
    Month? InMonth { get; }
    TimeSpan StartTime { get; }
    TimeSpan? Duration { get; }
}