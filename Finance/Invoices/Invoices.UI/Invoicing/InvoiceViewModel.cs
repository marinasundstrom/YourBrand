using System.ComponentModel.DataAnnotations;

using YourBrand.Invoices.Client;

namespace YourBrand.Invoices.Invoicing;

public class InvoiceViewModel
{
    public int Id { get; set; }

    [Required]
    public DateTime? Date { get; set; }

    [Required]
    public TimeSpan? Time { get; set; }

    [Required]
    public InvoiceStatus Status { get; set; }

    public string? Reference { get; set; }

    public string? Note { get; set; }

    public DateTime? DueDate { get; set; }

    public List<InvoiceItemViewModel> Items { get; set; } = new List<InvoiceItemViewModel>();

    public decimal SubTotal => Items.Sum(i => i.LineTotal);

    public decimal Vat => Items.Sum(i => i.LineTotal.GetVatFromSubTotal(i.VatRate));

    public decimal Total 
    {
        get 
        {
            var total = Items.Sum(i => i.LineTotal.AddVat(i.VatRate));
            total += DomesticService?.RequestedAmount.GetValueOrDefault() ?? 0;
            return total;
        }
    }

    public decimal? Paid { get; set; }

    public InvoiceDomesticServiceViewModel? DomesticService { get; set; }
}
