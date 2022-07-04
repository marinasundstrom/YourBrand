namespace OrderPriceCalculator;

public interface IOrderItem2 : IOrderItem, IHasChargesWithTotal, IHasDiscountsWithTotal
{
    decimal Vat { get; set; }

    decimal Total { get; set; }
}