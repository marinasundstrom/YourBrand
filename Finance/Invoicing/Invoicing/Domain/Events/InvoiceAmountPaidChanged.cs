using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Domain.Events;

public record InvoiceAmountPaidChanged : DomainEvent
{
    public InvoiceAmountPaidChanged(string invoiceId, decimal? amountPaid)
    {
        InvoiceId = invoiceId;
        AmountPaid = amountPaid;
    }

    public string InvoiceId { get; }

    public decimal? AmountPaid { get; }
}
