using System.ComponentModel.DataAnnotations;

using YourBrand.Invoicing.Client;

namespace YourBrand.Invoicing.Invoicing;

public class InvoiceItemViewModel
{
    public string? Id { get; set; }

    public ProductType ProductType { get; set; }

    [Required]
    public string Description { get; set; } = null!;

    public string? ItemId { get; set; } = null!;

    public string? Sku { get; set; } = null!;

    [Required]
    public decimal UnitPrice { get; set; }

    [Required]
    public string Unit { get; set; } = null!;

    public decimal? RegularPrice { get; set; }

    public decimal? Discount { get; set; }

    [Required]
    [Range(0.0001, double.MaxValue)]
    public double Quantity { get; set; } = 1;

    public double VatRate { get; set; } = 0.25;

    public decimal SubTotal => LineTotal.GetSubTotal(VatRate);

    public decimal Vat => LineTotal.GetVatFromTotal(VatRate);

    public decimal LineTotal => UnitPrice * (decimal)Quantity;

    public bool IsTaxDeductibleService { get; set; }

    public InvoiceItemDomesticServiceViewModel? DomesticService { get; set; }
}
