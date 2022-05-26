using Invoices.Domain.Common;

namespace Invoices.Domain.Events;

public class InvoiceNoteChanged : DomainEvent
{
    public InvoiceNoteChanged(int invoiceId, string note)
    {
        InvoiceId = invoiceId;
        Note = note;
    }

    public int InvoiceId { get; }

    public string Note { get; }
}