using YourBrand.Invoicing.Domain.Common;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Domain.Events;

public record InvoiceStatusChanged : DomainEvent
{
    public InvoiceStatusChanged(string invoiceId, int status)
    {
        InvoiceId = invoiceId;
        Status = status;
    }

    public string InvoiceId { get; }

    public int Status { get; }
}