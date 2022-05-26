using Invoices.Domain.Common;
using Invoices.Domain.Enums;

namespace Invoices.Domain.Events;

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
