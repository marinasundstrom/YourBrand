using YourBrand.Sales.Domain.Enums;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;

namespace YourBrand.Sales.Features.Subscriptions;

public class SubscriptionDto : Domain.Entities.ISubscriptionParameters
{
    public Guid Id { get; set; }
    public int SubscriptionNo { get; set; }
    public SubscriptionPlanShortDto Plan { get; set; }
    public SubscriptionStatus Status { get; set; }
    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public OrderShortDto? Order { get; set; }
    public string? OrderItemId { get; set; }

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