using System.ComponentModel.DataAnnotations;

using YourBrand.Invoices.Client;

namespace YourBrand.Invoices.Invoicing;

public class InvoiceDomesticServiceViewModel
{
    [Required]
    public DomesticServiceKind? Kind { get; set; }

    [Required]
    public string? Description { get; set; }

    [Required]
    public string? Buyer { get; set; }

    [Required]
    public decimal? RequestedAmount { get; set; }
}