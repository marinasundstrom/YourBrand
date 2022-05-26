using YourBrand.Invoices.Domain.Common;
using YourBrand.Invoices.Domain.Enums;

namespace YourBrand.Invoices.Domain.Events;

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
