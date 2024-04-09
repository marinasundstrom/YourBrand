using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Sales.Domain.Enums;
using YourBrand.Sales.Domain.ValueObjects;

namespace YourBrand.Sales.Domain.Entities;

public class Subscription : AggregateRoot<Guid>, ISoftDelete, ISubscriptionParameters, IHasTenant
{
    public Subscription() : base(Guid.NewGuid())
    {

    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public int SubscriptionNo { get; set; }

    public int? CustomerId { get; set; }

    public Order? Order { get; set; }

    public string? OrderId { get; set; }

    public OrderItem? OrderItem { get; set; }

    public string? OrderItemId { get; set; }

    public SubscriptionPlan? SubscriptionPlan { get; set; }

    public Guid? SubscriptionPlanId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public SubscriptionStatus Status { get; set; }

    public DateTimeOffset StatusDate { get; set; }

    public string? Note { get; set; }

    public List<Order> Orders { get; } = new List<Order>();

    public List<OrderItem> OrderItems { get; } = new List<OrderItem>();

    public DateTimeOffset? Deleted { get; set; }

    public UserId? DeletedById { get; set; }

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
    public bool AutoRenew { get; set; }
}