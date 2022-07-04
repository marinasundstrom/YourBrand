namespace OrderPriceCalculator;

public interface IOrder : IHasCharges, IHasDiscounts
{
    IEnumerable<IOrderItem> Items { get; }
}