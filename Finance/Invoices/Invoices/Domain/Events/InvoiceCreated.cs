using Invoices.Domain.Common;

namespace Invoices.Domain.Events;

public class InvoiceCreated : DomainEvent
{
    public InvoiceCreated(int invoiceId)
    {
        InvoiceId = invoiceId;
    }

    public int InvoiceId { get; }
}