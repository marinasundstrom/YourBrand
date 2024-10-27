using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Sales.Domain.Enums;
using YourBrand.Sales.Domain.ValueObjects;

namespace YourBrand.Sales.Domain.Entities;

public class Subscription : AggregateRoot<Guid>, IAuditable, ISoftDeletable, ISubscriptionParameters, IHasTenant, IHasOrganization
{
    public Subscription() : base(Guid.NewGuid())
    {
        TypeId = 1;
        StatusId = 1;
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public int SubscriptionNo { get; set; }

    public SubscriptionType Type { get; set; } = null!;
    public int TypeId { get; set; }

    public int? CustomerId { get; set; }

    public SubscriptionStatus Status { get; set; } = null!;
    public int StatusId { get; set; }
    public DateTimeOffset StatusDate { get; set; }

    public void UpdateStatus(int statusId)
    {
        StatusId = statusId;
        StatusDate = DateTimeOffset.UtcNow;
    }

    public string? Note { get; set; }

    public Order? Order { get; set; }
    public string? OrderId { get; set; }

    public OrderItem? OrderItem { get; set; }
    public string? OrderItemId { get; set; }

    public List<Order> Orders { get; } = new List<Order>();
    public List<OrderItem> OrderItems { get; } = new List<OrderItem>();

    public SubscriptionPlan? SubscriptionPlan { get; set; }
    public Guid? SubscriptionPlanId { get; set; }

    public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public DateTime? CancellationDate { get; set; }
    public DateTime? RenewalDate { get; set; }
    public bool AutoRenew { get; set; }


    // The date when the subscription was canceled (nullable if not canceled)

    // Optional reason code or message explaining why the subscription was canceled
    public string? CancellationReason { get; set; }

    // Optional information about who or what triggered the cancellation (e.g., customer, system)
    public string? CancellationInitiator { get; set; }

    // Method to cancel the subscription, setting the relevant properties
    public void Cancel(string reason, string initiator)
    {
        StatusId = 8;
        CancellationDate = DateTime.Now;
        CancellationReason = reason;
        CancellationInitiator = initiator;
    }

    public DateTime? TrialStartDate { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public DateTime? TrialEndedDate { get; set; }

    public DateOnly? NextBillingDate { get; set; }
    public BillingStatus BillingStatus { get; set; }

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

    public User? CreatedBy { get; set; }
    public UserId? CreatedById { get; set; }
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }

    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
}