using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Domain.Events;

public record InvoiceDueDateChanged : DomainEvent
{
    public InvoiceDueDateChanged(string invoiceId, DateTimeOffset? dueDate)
    {
        InvoiceId = invoiceId;
        DueDate = dueDate;
    }

    public string InvoiceId { get; }

    public DateTimeOffset? DueDate { get; }
}