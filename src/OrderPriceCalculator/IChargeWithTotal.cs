namespace OrderPriceCalculator;

public interface IChargeWithTotal : ICharge
{
    /// <summary>
    /// Gets or sets the total charge.
    /// </summary>
    decimal Total { get; set; }
}