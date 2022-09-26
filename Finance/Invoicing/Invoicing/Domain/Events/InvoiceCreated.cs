using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Domain.Events;

public class InvoiceCreated : DomainEvent
{
    public InvoiceCreated(string invoiceId)
    {
        InvoiceId = invoiceId;
    }

    public string InvoiceId { get; }
}