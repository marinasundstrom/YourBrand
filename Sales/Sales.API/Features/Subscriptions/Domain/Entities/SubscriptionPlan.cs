using YourBrand.Sales.API.Features.OrderManagement.Domain.Entities;
using YourBrand.Sales.Domain.Enums;

namespace YourBrand.Sales.Domain.Entities;

public class SubscriptionPlan : AggregateRoot<Guid>, ISoftDelete, ISubscriptionParameters
{
    public SubscriptionPlan() : base(Guid.NewGuid())
    {

    }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? CustomerId { get; set; } // This Subscription belongs to a Customer

    public string? ItemId { get; set; } // This Subscription belongs to a Product

    public decimal? Price { get; set; }

    //public CurrencyAmount? Price { get; set; }

    public bool AutoRenew { get; set; }

    public Recurrence Recurrence { get; set; }

    //public bool RescheduleWhenOnWeekend { get; set; } = true;

    public int? EveryDays { get; set; }

    public int? EveryWeeks { get; set; }

    public WeekDays? OnWeekDays { get; set; }

    public int? EveryMonths { get; set; } // Every two months

    public int? EveryYears { get; set; }

    public int? OnDay { get; set; } // 3rd of January. If OnDayOfWeek set to i.e. Tuesday, 3rd Tuesday

    public DayOfWeek? OnDayOfWeek { get; set; }

    public Month? InMonth { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan? Duration { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public string? DeletedById { get; set; }

    public SubscriptionPlan WithName(string name)
    {
        Name = name;

        return this;
    }

    public SubscriptionPlan WithDescription(string description)
    {
        Description = description;

        return this;
    }

    public SubscriptionPlan WithEndTime(TimeSpan value)
    {
        Duration = value - StartTime;

        return this;
    }
}