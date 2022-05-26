using Invoices.Domain.Enums;

namespace Invoices.Domain.Entities;

public class InvoiceItem 
{
    public InvoiceItem(ProductType productType, string description, decimal unitPrice, string unit, double vatRate, double quantity)
    {
        ProductType = productType;
        Description = description;
        Unit = unit;
        UpdateUnitPrice(unitPrice, vatRate);
        UpdateQuantity(quantity);
    }

    public int Id { get; private set; }

    public Invoice Invoice { get; set; } = null!;

    public int InvoiceId { get; private set; }

    public string? ProductId { get; private set; }

    public ProductType ProductType  { get; private set; }

    public string Description { get; private set; } = null!;

    public void UpdateDescription(string description) 
    {
        Description = description;
    }

    public string Unit { get; set; }

    public double Quantity { get; private set; }

    public void UpdateQuantity(double quantity) 
    {
        Quantity = quantity;

        LineTotal = UnitPrice * (decimal)Quantity;

        Invoice?.UpdateTotals();
    }
    
    public decimal UnitPrice { get; private set; }

    public double VatRate { get; private set; }

    public void UpdateUnitPrice(decimal unitPrice, double vatRate) 
    {
        UnitPrice = unitPrice;
        VatRate = vatRate;

        LineTotal = UnitPrice * (decimal)Quantity;

        Invoice?.UpdateTotals();
    }

    public decimal LineTotal { get; set; }

    public decimal Vat()
    {
         return LineTotal - SubTotal();
    }

    public decimal SubTotal()
    {
        return LineTotal / (1m + (decimal)VatRate);
    }
}