using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Sales.Domain.Enums;
using YourBrand.Sales.Domain.ValueObjects;

namespace YourBrand.Sales.Domain.Entities;

public class SubscriptionPlan : AggregateRoot<Guid>, IAuditableEntity<Guid, User>, ISoftDeletable, ISubscriptionParameters, IHasTenant
{
    public SubscriptionPlan() : base(Guid.NewGuid())
    {

    }


    public TenantId TenantId { get; set; }

    public SubscriptionPlanType PlanType { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? CustomerId { get; set; } // This Subscription belongs to a Customer

    public int? ProductId { get; set; } // This Subscription belongs to a Product

    public string? ItemId { get; set; } // This Subscription belongs to a Product

    public SubscriptionSchedule Schedule { get; set; }

    public RenewalOption RenewalOption { get; set; }
    public RenewalInterval RenewalCycle { get; set; } = RenewalInterval.Months;
    public int RenewalPeriod { get; set; } = 12; // 12 months for yearly renewals, for example

    public TimeSpan? CancellationFinalizationPeriod { get; set; }

    public double? DiscountPercentage { get; } // Discount percentage
    public decimal? FixedDiscountAmount { get; } // Fixed discount amount

    // Add TrialPeriod as an owned type
    public TrialPeriod Trial { get; private set; } = new TrialPeriod(false, 0);

    public decimal GetSubscriptionPrice(decimal basePrice)
    {
        if (FixedDiscountAmount.HasValue)
        {
            return Math.Max(basePrice - FixedDiscountAmount.Value, 0);
        }
        else if (DiscountPercentage.HasValue)
        {
            return basePrice * (1 - (decimal)DiscountPercentage.Value / 100);
        }

        return basePrice;
    }

    public decimal GetSavings(decimal basePrice)
    {
        return basePrice - GetSubscriptionPrice(basePrice);
    }

    // Delegate trial price calculation to the TrialPeriod owned type
    public decimal GetTrialPrice(decimal basePrice)
    {
        return Trial.GetTrialPrice(basePrice);
    }

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

    public SubscriptionPlan WithTrial(int days, decimal? trialDiscountPercentage = null, decimal? trialFixedDiscountAmount = null)
    {
        Trial = new TrialPeriod(true, days, trialDiscountPercentage, trialFixedDiscountAmount);
        return this;
    }

    public SubscriptionPlan WithAutoRenewal()
    {
        RenewalOption = RenewalOption.Automatic;

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

public enum RenewalOption
{
    Automatic = 1,
    Manual = 0
}

public enum RenewalInterval
{
    Days,
    Weeks,
    Months,
    Years
}

public class TrialPeriod
{
    private TrialPeriod() { }

    public TrialPeriod(bool hasTrial, int length, decimal? discountPercentage = null, decimal? fixedDiscountAmount = null)
    {
        HasTrial = hasTrial;
        Length = length;
        DiscountPercentage = discountPercentage;
        FixedDiscountAmount = fixedDiscountAmount;
    }

    public bool HasTrial { get; private set; }
    public int Length { get; private set; } // Trial length in days
    public decimal? DiscountPercentage { get; private set; }
    public decimal? FixedDiscountAmount { get; private set; }

    // Calculate the trial price based on base price and trial discounts
    public decimal GetTrialPrice(decimal basePrice)
    {
        if (!HasTrial)
        {
            return basePrice; // No trial discount if HasTrial is false
        }

        if (FixedDiscountAmount.HasValue)
        {
            return Math.Max(basePrice - FixedDiscountAmount.Value, 0);
        }

        if (DiscountPercentage.HasValue)
        {
            return basePrice * (1 - DiscountPercentage.Value / 100);
        }

        return basePrice; // No discount applied if both values are null
    }
}