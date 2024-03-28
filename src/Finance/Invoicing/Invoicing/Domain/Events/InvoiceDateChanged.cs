using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Domain.Events;

public record InvoiceDateChanged : DomainEvent
{
    public InvoiceDateChanged(string invoiceId, DateTime? date)
    {
        InvoiceId = invoiceId;
        Date = date;
    }

    public string InvoiceId { get; }

    public DateTime? Date { get; }
}
