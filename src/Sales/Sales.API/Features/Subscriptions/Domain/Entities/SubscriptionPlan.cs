using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Sales.Domain.Enums;
using YourBrand.Sales.Domain.ValueObjects;

namespace YourBrand.Sales.Domain.Entities;

public class SubscriptionPlan : AggregateRoot<Guid>, IAuditable, ISoftDeletable, ISubscriptionParameters, IHasTenant
{
    public SubscriptionPlan() : base(Guid.NewGuid())
    {

    }


    public TenantId TenantId { get; set; }

    public SubscriptionPlanType PlanType { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? CustomerId { get; set; } // This Subscription belongs to a Customer

    public string? ItemId { get; set; } // This Subscription belongs to a Product

    public decimal? Price { get; set; }

    public TimeSpan? CancellationFinalizationPeriod { get; set; }

    public bool HasTrial { get; set; }
    public int TrialLength { get; set; }

    public bool AutoRenew { get; set; }

    public SubscriptionSchedule Schedule { get; set; }

    public User? CreatedBy { get; set; }
    public UserId? CreatedById { get; set; }
    public DateTimeOffset Created { get; set; }
    public User? LastModifiedBy { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }

    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }

    public static SubscriptionPlan Create(SubscriptionPlanType planType, string name, string? description = null)
    {
        return new SubscriptionPlan()
        {
            PlanType = planType,
            Name = name,
            Description = description
        };
    }

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

    public SubscriptionPlan WithSchedule(SubscriptionSchedule schedule)
    {
        Schedule = schedule;

        return this;
    }

    public SubscriptionPlan WithEndTime(TimeOnly value)
    {
        Schedule.Duration = value - Schedule.StartTime;

        return this;
    }

    public SubscriptionPlan WithTrial(int days)
    {
        HasTrial = true;
        TrialLength = days;

        return this;
    }

    public SubscriptionPlan WithAutoRenewal()
    {
        AutoRenew = true;

        return this;
    }

    public SubscriptionPlan WithCancellationFinalizationPeriod(TimeSpan timeSpan)
    {
        CancellationFinalizationPeriod = timeSpan;

        return this;
    }
}

public enum SubscriptionPlanType
{
    RecurringOrder = 1,
    RecurringBilling = 2,
}