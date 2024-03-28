namespace OrderPriceCalculator;

public interface IHasDiscountsWithTotal : IHasDiscounts
{
    new IEnumerable<IDiscountWithTotal> Discounts { get; }
}