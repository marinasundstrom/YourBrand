using Invoices.Domain.Common;
using Invoices.Domain.Enums;

namespace Invoices.Domain.Events;

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
