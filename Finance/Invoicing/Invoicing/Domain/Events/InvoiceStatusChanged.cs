using YourBrand.Invoicing.Domain.Common;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Domain.Events;

public class InvoiceStatusChanged : DomainEvent
{
    public InvoiceStatusChanged(int invoiceId, InvoiceStatus status)
    {
        InvoiceId = invoiceId;
        Status = status;
    }

    public int InvoiceId { get; }

    public InvoiceStatus Status { get; }
}
