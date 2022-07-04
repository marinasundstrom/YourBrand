namespace OrderPriceCalculator;

public interface IDiscount
{
    /// <summary>
    /// Gets or sets the description for this discount.
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// Gets or sets the optional quantity to which this Discount applies. If null the discount will apply to every unit.
    /// </summary>
    int? Quantity { get; set; }

    /// <summary>
    /// Gets or sets the maximimum number of times this discount will be applied.
    /// </summary>
    int? Limit { get; set; }

    /// <summary>
    /// Gets or sets the discounted amount.
    /// </summary>
    decimal? Amount { get; set; }

    /// <summary>
    /// Gets or sets the discounted percentage.
    /// </summary>
    double? Percent { get; set; }
}