using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Domain.Events;

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
