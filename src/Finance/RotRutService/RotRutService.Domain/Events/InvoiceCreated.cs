using YourBrand.RotRutService.Domain.Common;

namespace YourBrand.RotRutService.Domain.Events;

public record InvoiceCreated : DomainEvent
{
    public InvoiceCreated(int invoiceId)
    {
        InvoiceId = invoiceId;
    }

    public int InvoiceId { get; }
}