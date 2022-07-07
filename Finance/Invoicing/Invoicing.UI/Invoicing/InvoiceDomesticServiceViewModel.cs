using System.ComponentModel.DataAnnotations;

using YourBrand.Invoicing.Client;

namespace YourBrand.Invoicing.Invoicing;

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