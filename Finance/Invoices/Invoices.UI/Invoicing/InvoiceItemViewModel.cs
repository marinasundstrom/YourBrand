using System.ComponentModel.DataAnnotations;

using YourBrand.Invoices.Client;

namespace YourBrand.Accounting.Client.Invoicing;

public class InvoiceItemViewModel
{
    public int Id { get; set; }

    [Required]
    public string Description { get; set; } = null!;

    public ProductType ProductType { get; set; }

    [Required]
    public decimal UnitPrice { get; set; }

    [Required]
    public string Unit { get; set; } = null!;

    [Required]
    [Range(0.0001, double.MaxValue)]
    public double Quantity { get; set; } = 1;

    public double VatRate { get; set; } = 0.25;

    [Required]
    public decimal SubTotal => LineTotal.SubTotal(VatRate);

    [Required]
    public decimal Vat => LineTotal.Vat(VatRate);

    public decimal LineTotal => UnitPrice * (decimal)Quantity;

    public bool IsTaxDeductableService { get; set; }

    public DomesticServiceKind Kind { get; set; }

    public HomeRepairAndMaintenanceServiceType? HomeRepairAndMaintenanceServiceType { get; set; }

    public HouseholdServiceType? HouseholdServiceType { get; set; }

    // public double? Hours { get; set; }

    // public decimal? LaborCost { get; set; }

    // public decimal? MaterialCost { get; set; }

    // public decimal? OtherCosts { get; set; }

    // public decimal? RequestedAmount { get; set; }
}