using YourBrand.Invoices.Domain.Common;
using YourBrand.Invoices.Domain.Enums;

namespace YourBrand.Invoices.Domain.Events;

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
