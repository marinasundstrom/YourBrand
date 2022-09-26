using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Domain.Events;

public class InvoiceDueDateChanged : DomainEvent
{
    public InvoiceDueDateChanged(string invoiceId, DateTime? dueDate)
    {
        InvoiceId = invoiceId;
        DueDate = dueDate;
    }

    public string InvoiceId { get; }

    public DateTime? DueDate { get; }
}
