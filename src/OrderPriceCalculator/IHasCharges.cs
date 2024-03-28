namespace OrderPriceCalculator;

public interface IHasCharges
{
    /// <summary>
    /// Gets the Charges.
    /// </summary>
    IEnumerable<ICharge> Charges { get; }

    /// <summary>
    /// Gets or sets the charged amount.
    /// </summary>
    decimal? Charge { get; set; }
}