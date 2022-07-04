namespace OrderPriceCalculator;

public interface IHasChargesWithTotal : IHasCharges
{
    new IEnumerable<IChargeWithTotal> Charges { get; }
}