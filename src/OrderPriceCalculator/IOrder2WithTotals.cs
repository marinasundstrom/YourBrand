namespace OrderPriceCalculator;

public interface IOrder2WithTotals : IOrder2
{
    IEnumerable<IOrderTotals> Totals { get; }
}