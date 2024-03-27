using System;
using System.Collections.Generic;

using YourBrand.Sales.API.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Domain.Enums;

namespace YourBrand.Sales.Features.Subscriptions;

public class SubscriptionDto : Domain.Entities.ISubscriptionParameters
{
    public Guid Id { get; set; }
    public SubscriptionPlanShortDto Plan { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get;  set; }
    public OrderShortDto? Order { get;  set; }
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
    public TimeSpan StartTime { get; set; }
    public TimeSpan? Duration { get; set; }
}