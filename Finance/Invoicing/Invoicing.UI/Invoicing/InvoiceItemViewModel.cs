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

    public decimal? RegularPrice { get; set; }

    public decimal? Discount => RegularPrice is null ? null : RegularPrice.GetValueOrDefault() - UnitPrice;

    [Required]
    [Range(0.0001, double.MaxValue)]
    public double Quantity { get; set; } = 1;

    [Required]
    public string Unit { get; set; } = null!;

    public double VatRate { get; set; } = 0.25;

    public decimal SubTotal => LineTotal.GetSubTotal(VatRate);

    public decimal Vat => Math.Round(LineTotal.GetVatFromTotal(VatRate), 2, MidpointRounding.AwayFromZero);

    public decimal LineTotal => Math.Round(UnitPrice * (decimal)Quantity, 2, MidpointRounding.AwayFromZero);

    public string? Notes { get; set; }

    public bool IsTaxDeductibleService { get; set; }

    public InvoiceItemDomesticServiceViewModel? DomesticService { get; set; }
}
