using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Domain.Events;

public record InvoiceDateChanged : DomainEvent
{
    public InvoiceDateChanged(string invoiceId, DateTimeOffset? date)
    {
        InvoiceId = invoiceId;
        Date = date;
    }

    public string InvoiceId { get; }

    public DateTimeOffset? Date { get; }
}