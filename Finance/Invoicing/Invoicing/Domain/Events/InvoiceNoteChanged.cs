using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Domain.Events;

public record InvoiceNoteChanged : DomainEvent
{
    public InvoiceNoteChanged(string invoiceId, string note)
    {
        InvoiceId = invoiceId;
        Note = note;
    }

    public string InvoiceId { get; }

    public string Note { get; }
}