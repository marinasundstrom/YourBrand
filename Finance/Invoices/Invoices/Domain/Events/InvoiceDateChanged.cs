using YourBrand.Invoices.Domain.Common;

namespace YourBrand.Invoices.Domain.Events;

public class InvoiceDateChanged : DomainEvent
{
    public InvoiceDateChanged(int invoiceId, DateTime? date)
    {
        InvoiceId = invoiceId;
        Date = date;
    }

    public int InvoiceId { get; }

    public DateTime? Date { get; }
}
