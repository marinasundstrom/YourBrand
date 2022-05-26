using System.ComponentModel.DataAnnotations;

using Invoices.Client;

namespace Accounting.Client.Invoicing;

public class InvoiceItemViewModel
{
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
}