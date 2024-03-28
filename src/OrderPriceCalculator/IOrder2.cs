namespace OrderPriceCalculator;

public interface IOrder2 : IOrder, IHasChargesWithTotal, IHasDiscountsWithTotal
{
    decimal? SubTotal { get; set; }
    double? VatRate { get; set; }
    decimal? Vat { get; set; }

    decimal? Rounding { get; set; }
    decimal Total { get; set; }

    new IEnumerable<IOrderItem2> Items { get; }
}