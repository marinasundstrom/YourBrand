using YourBrand.Invoices.Domain.Common;

namespace YourBrand.Invoices.Domain.Events;

public class InvoiceCreated : DomainEvent
{
    public InvoiceCreated(int invoiceId)
    {
        InvoiceId = invoiceId;
    }

    public int InvoiceId { get; }
}