using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Domain.Events;

public class InvoiceCreated : DomainEvent
{
    public InvoiceCreated(int invoiceId)
    {
        InvoiceId = invoiceId;
    }

    public int InvoiceId { get; }
}