namespace OrderPriceCalculator;

public interface IOrderItem : IHasCharges, IHasDiscounts
{
    string Description { get; set; }
    decimal Price { get; set; }
    double VatRate { get; set; }
    double Quantity { get; set; }
}