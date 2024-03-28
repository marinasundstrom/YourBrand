namespace OrderPriceCalculator;

public interface ICharge
{
    /// <summary>
    /// Gets or sets the description for this charge.
    /// </summary>
    string Description { get; set; }

    /// <summary>
    /// Gets or sets the optional quantity to which this charge applies. If null the charge will apply to every unit.
    /// </summary>
    int? Quantity { get; set; }

    /// <summary>
    /// Gets or sets the maximimum number of times this charge will be applied.
    /// </summary>
    int? Limit { get; set; }

    /// <summary>
    /// Gets or sets the charged amount.
    /// </summary>
    decimal? Amount { get; set; }

    /// <summary>
    /// Gets or sets the charged percentage.
    /// </summary>
    double? Percent { get; set; }
}