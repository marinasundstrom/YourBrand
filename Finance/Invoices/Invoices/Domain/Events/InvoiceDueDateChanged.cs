using Invoices.Domain.Common;

namespace Invoices.Domain.Events;

public class InvoiceDueDateChanged : DomainEvent
{
    public InvoiceDueDateChanged(int invoiceId, DateTime? dueDate)
    {
        InvoiceId = invoiceId;
        DueDate = dueDate;
    }

    public int InvoiceId { get; }

    public DateTime? DueDate { get; }
}
