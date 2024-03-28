namespace OrderPriceCalculator;

public interface IHasDiscounts
{
    /// <summary>
    /// Gets the Discounts.
    /// </summary>
    IEnumerable<IDiscount> Discounts { get; }

    /// <summary>
    /// Gets or sets the discounted amount.
    /// </summary>
    decimal? Discount { get; set; }
}