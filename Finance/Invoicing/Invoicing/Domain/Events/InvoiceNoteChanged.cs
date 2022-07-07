using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Domain.Events;

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