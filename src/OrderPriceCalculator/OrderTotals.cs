namespace OrderPriceCalculator;

public class OrderTotals : IOrderTotals
{
    public int Id { get; set; }
    public double VatRate { get; set; }
    public decimal SubTotal { get; set; }
    public decimal Vat { get; set; }
    public decimal Total { get; set; }
}