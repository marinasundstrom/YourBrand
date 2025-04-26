using Microsoft.EntityFrameworkCore;

using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public class ProductSubscriptionPlan : Entity<string>, IHasTenant, IAuditableEntity<string>, ISoftDeletableWithAudit, IHasOrganization
{
    private ProductSubscriptionPlan() { }

    public ProductSubscriptionPlan(
        string title,
        string? description,
        TimeInterval subscriptionCycle,
        TimeInterval billingCycle,
        RenewalOption renewal,
        RenewalInterval renewalCycle,
        int renewalPeriod,
        TimeSpan? cancellationFinalizationPeriod,
        double? discountPercentage = null,
        decimal? fixedDiscountAmount = null,
        TrialPeriod? trialPeriod = null) : base(Guid.NewGuid().ToString())
    {
        Title = title ?? throw new ArgumentNullException(nameof(title));
        SubscriptionCycle = subscriptionCycle;
        BillingCycle = billingCycle;
        RenewalCycle = renewalCycle;
        RenewalPeriod = renewalPeriod;
        RenewalOption = renewal;
        CancellationFinalizationPeriod = cancellationFinalizationPeriod;
        DiscountPercentage = discountPercentage;
        FixedDiscountAmount = fixedDiscountAmount;
        Trial = trialPeriod ?? new TrialPeriod(false, 0);

        if (DiscountPercentage < 0 || DiscountPercentage > 100)
            throw new ArgumentOutOfRangeException(nameof(DiscountPercentage), "Must be between 0 and 100");

        if (FixedDiscountAmount < 0)
            throw new ArgumentOutOfRangeException(nameof(FixedDiscountAmount), "Cannot be negative");
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public int ProductId { get; private set; }

    public Product Product { get; private set; }

    public string Title { get; private set; }
    public string? Description { get; private set; }

    public bool IsActive { get; private set; }

    public TimeInterval SubscriptionCycle { get; private set; } // E.g., Monthly, Quarterly, Yearly
    public TimeInterval BillingCycle { get; private set; } // E.g., Monthly, Quarterly, Yearly
    public RenewalOption RenewalOption { get; private set; } // E.g., Automatic or Manual
    public RenewalInterval RenewalCycle { get; set; } = RenewalInterval.Months;
    public int RenewalPeriod { get; set; } = 12; // 12 months for yearly renewals, for example

    public TimeSpan? CancellationFinalizationPeriod { get; set; }

    public double? DiscountPercentage { get; private set; } // Discount percentage
    public decimal? FixedDiscountAmount { get; private set; } // Fixed discount amount

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

    public UserId? CreatedById { get; set; }
    public DateTimeOffset Created { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
}

public enum TimeInterval
{
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Yearly
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