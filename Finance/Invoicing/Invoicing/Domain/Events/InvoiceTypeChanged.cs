using YourBrand.Invoicing.Domain.Common;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Domain.Events;

public class InvoiceTypeChanged : DomainEvent
{
    public InvoiceTypeChanged(int invoiceId, InvoiceType type)
    {
        InvoiceId = invoiceId;
        Type = type;
    }

    public int InvoiceId { get; }

    public InvoiceType Type { get; }
}
