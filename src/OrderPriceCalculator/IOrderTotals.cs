namespace OrderPriceCalculator;

public interface IOrderTotals
{
    double VatRate { get; set; }
    decimal SubTotal { get; set; }
    decimal Vat { get; set; }
    decimal Total { get; set; }
}