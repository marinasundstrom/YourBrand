using YourBrand.Marketing.Domain.Enums;

namespace YourBrand.Marketing.Domain.Entities;

public class Discount : Entity<string>, IAuditable
{

#nullable disable

    protected Discount()
    {

    }

#nullable restore

    public static Discount CreateDiscountForItem(string itemId, double percentage, decimal amount)
    {
        return new Discount
        {
            ItemId = itemId,
            Percentage = percentage,
            Amount = amount
        };
    }

    public static Discount CreateDiscountForGroup(string itemGroupId, double percentage, decimal amount)
    {
        return new Discount
        {
            ItemGroupId = itemGroupId,
            Percentage = percentage,
            Amount = amount
        };
    }

    public static Discount CreateDiscountForPurchase(double percentage, decimal amount)
    {
        return new Discount
        {
            Percentage = percentage,
            Amount = amount
        };
    }

    public DiscountType Type { get; set; }

    public Contact? Contact { get; set; }
    public string? ContactId { get; set; }

    public string? CustomerId { get; set; }

    public string? Code { get; set; }

    public string? ItemId { get; set; } = null!;
    public string? ItemName { get; set; }
    public string? ItemDescription { get; }

    public string? ItemGroupId { get; set; }

    public double? Percentage { get; set; }
    public decimal? Amount { get; set; }

    public string? CreatedById { get; set; } = null!;
    public DateTimeOffset Created { get; set; }
    public string? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }
}