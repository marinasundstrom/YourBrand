namespace OrderPriceCalculator;

public class Discount : IDiscountWithTotal
{
    public string Description { get; set; } = null!;
    public int? Quantity { get; set; }
    public int? Limit { get; set; }
    public decimal? Amount { get; set; }
    public double? Percent { get; set; }
    public decimal Total { get; set; }
}