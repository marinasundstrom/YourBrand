using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public class SubscriptionPlan : Entity<string>, IHasTenant
{
    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public int ProductId { get; set; }

    public string Title { get; set; }
    public string? Description { get; set; }

    public decimal? DiscountPercentage { get; set; } // Discount percentage
    public decimal? FixedDiscountAmount { get; set; } // Fixed discount amount
    public BillingFrequency BillingFrequency { get; set; } // E.g., Monthly, Quarterly, Yearly
    public int DurationInMonths { get; set; } // Length of the subscription in months
    public RenewalType RenewalType { get; set; } // E.g., Automatic or Manual
    public bool IsActive { get; set; }

    // Trial-related properties
    public bool HasTrial { get; set; } // Indicates if the plan includes a trial period
    public int TrialDurationInDays { get; set; } // Duration of the trial in days
    public decimal? TrialPrice { get; set; } // Price during the trial period (could be free)

    // Calculate the subscription price based on base price and discount
    public decimal GetSubscriptionPrice(decimal basePrice)
    {
        if (DiscountPercentage.HasValue)
        {
            return basePrice - (basePrice * (DiscountPercentage.Value / 100));
        }
        else if (FixedDiscountAmount.HasValue)
        {
            return basePrice - FixedDiscountAmount.Value;
        }
        else
        {
            return basePrice; // If no discount is provided, use the base price
        }
    }

    // Calculate savings based on base price and discount
    public decimal GetSavings(decimal basePrice)
    {
        return basePrice - GetSubscriptionPrice(basePrice);
    }

    // Get trial price (returns 0 if it's a free trial or the defined trial price)
    public decimal GetTrialPrice()
    {
        return TrialPrice ?? 0m; // Return 0 if TrialPrice is null, indicating a free trial
    }

    // Constructor
    /*
    public SubscriptionPlan(int subscriptionPlanId, int productId, string billingFrequency, int durationInMonths, string renewalType, bool isActive, decimal? discountPercentage = null, decimal? fixedDiscountAmount = null)
    {
        SubscriptionPlanId = subscriptionPlanId;
        ProductId = productId;
        BillingFrequency = billingFrequency;
        DurationInMonths = durationInMonths;
        RenewalType = renewalType;
        IsActive = isActive;
        DiscountPercentage = discountPercentage;
        FixedDiscountAmount = fixedDiscountAmount;
    } */
}

public enum BillingFrequency
{
    Daily,
    Weekly,
    Monthly,
    Quarterly,
    Yearly
}

public enum RenewalType
{
    Automatic,
    Manual
}