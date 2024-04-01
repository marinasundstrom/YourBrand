using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Domain.Events;

public record InvoiceReferenceChanged : DomainEvent
{
    public InvoiceReferenceChanged(string invoiceId, string reference)
    {
        InvoiceId = invoiceId;
        Reference = reference;
    }

    public string InvoiceId { get; }

    public string Reference { get; }
}