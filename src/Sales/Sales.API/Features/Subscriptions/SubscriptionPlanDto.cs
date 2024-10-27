using YourBrand.Sales.Domain.Enums;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public class SubscriptionPlanDto : Domain.Entities.ISubscriptionParameters
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? ProductId { get; set; }
    public decimal? Price { get; set; }
    
    public bool HasTrial { get; set; }
    public TimeSpan TrialLength { get; set; }

    public bool AutoRenew { get; set; }
    public Recurrence Recurrence { get; set; }
    public int? EveryDays { get; set; }
    public int? EveryWeeks { get; set; }
    public WeekDays? OnWeekDays { get; set; }
    public int? EveryMonths { get; set; }
    public int? EveryYears { get; set; }
    public int? OnDay { get; set; }
    public DayOfWeek? OnDayOfWeek { get; set; }
    public Month? InMonth { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeSpan? Duration { get; set; }
}

public class SubscriptionPlanShortDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string? ProductId { get; set; }
    public decimal? Price { get; set; }
}