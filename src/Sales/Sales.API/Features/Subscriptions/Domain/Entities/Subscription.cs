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
        StatusId = (int)SubscriptionStatusEnum.Pending;

        CancellationStatus = CancellationStatus.None;
        PaymentStatus = PaymentStatus.None;
        RenewalStatus = RenewalStatus.None;
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

    public DateOnly StartDate { get; set; } // The start date of the subscription
    public DateOnly? EndDate { get; set; } // Nullable, only set when the subscription ends

    public CancellationStatus CancellationStatus { get; set; } // Current cancellation status    
    public DateTime? CancellationDate { get; set; }
    public PaymentStatus PaymentStatus { get; set; } // Current payment status
    public RenewalStatus RenewalStatus { get; set; } // Current renewal status
    public DateTime? RenewalDate { get; set; }
    public bool AutoRenew { get; set; }

    // The date when the subscription was canceled (nullable if not canceled)

    // Optional reason code or message explaining why the subscription was canceled
    public string? CancellationReason { get; set; }

    // Optional information about who or what triggered the cancellation (e.g., customer, system)
    public string? CancellationInitiator { get; set; }

    public void Activate()
    {
        UpdateStatus((int)SubscriptionStatusEnum.Active);
    }

    public void Pause()
    {
        if (StatusId == (int)SubscriptionStatusEnum.Active)
        {
            UpdateStatus((int)SubscriptionStatusEnum.Paused);
        }
    }

    public void Cancel(string reason, string initiator)
    {
        UpdateStatus((int)SubscriptionStatusEnum.Canceled);
        CancellationStatus = CancellationStatus.Canceled;
        CancellationDate = DateTime.Now;
        CancellationReason = reason;
        CancellationInitiator = initiator;
        EndDate = DateOnly.FromDateTime(DateTime.Now);
    }

    public void RequestCancellation()
    {
        CancellationStatus = CancellationStatus.CancellationRequested;
    }

    public void Renew()
    {
        if (RenewalStatus == RenewalStatus.RenewalPending)
        {
            RenewalStatus = RenewalStatus.Renewed;
            UpdateStatus((int)SubscriptionStatusEnum.Active);
            NextBillingDate = DateTime.Now.AddMonths(1); // Example for monthly billing
        }
    }

    public void Suspend()
    {
        if (StatusId == (int)SubscriptionStatusEnum.Active || StatusId == (int)SubscriptionStatusEnum.Paused)
        {
            UpdateStatus((int)SubscriptionStatusEnum.Suspended);
        }
    }

    public void SetPaymentStatus(PaymentStatus paymentStatus)
    {
        PaymentStatus = paymentStatus;
        if (paymentStatus == PaymentStatus.PaymentSucceeded)
        {
            UpdateStatus((int)SubscriptionStatusEnum.Active);
            RenewalStatus = RenewalStatus.Renewed;
        }
        else if (paymentStatus == PaymentStatus.PaymentFailed)
        {
            UpdateStatus((int)SubscriptionStatusEnum.Suspended);
        }
    }

    public DateTime? TrialStartDate { get; set; }
    public DateTime? TrialEndDate { get; set; }
    public DateTime? TrialEndedDate { get; set; }

    public DateTime? NextBillingDate { get; set; }
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

public enum SubscriptionStatusEnum
{
    Pending = 1,
    Active,
    Trial,
    Paused,
    Canceled,
    Expired,
    Suspended
}


public enum CancellationStatus
{
    None,
    CancellationRequested,
    Canceled
}

public enum PaymentStatus
{
    None,
    PaymentPending,
    PaymentFailed,
    PaymentSucceeded
}

public enum RenewalStatus
{
    None,
    RenewalPending,
    RenewalFailed,
    Renewed
}
