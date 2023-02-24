using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Domain.Events;

public record InvoiceCreated : DomainEvent
{
    public InvoiceCreated(string invoiceId)
    {
        InvoiceId = invoiceId;
    }

    public string InvoiceId { get; }
}