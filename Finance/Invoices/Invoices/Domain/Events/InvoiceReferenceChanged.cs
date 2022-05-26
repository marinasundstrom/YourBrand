using Invoices.Domain.Common;

namespace Invoices.Domain.Events;

public class InvoiceReferenceChanged : DomainEvent
{
    public InvoiceReferenceChanged(int invoiceId, string reference)
    {
        InvoiceId = invoiceId;
        Reference = reference;
    }

    public int InvoiceId { get; }

    public string Reference { get; }
}
