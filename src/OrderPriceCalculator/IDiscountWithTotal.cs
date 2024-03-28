namespace OrderPriceCalculator;

public interface IDiscountWithTotal : IDiscount
{
    /// <summary>
    /// Gets or sets the total charge.
    /// </summary>
    decimal Total { get; set; }
}