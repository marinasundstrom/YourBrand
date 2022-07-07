using YourBrand.Invoicing.Domain.Common;

namespace YourBrand.Invoicing.Domain.Events;

public class InvoiceAmountPaidChanged : DomainEvent
{
    public InvoiceAmountPaidChanged(int invoiceId, decimal? amountPaid)
    {
        InvoiceId = invoiceId;
        AmountPaid = amountPaid;
    }

    public int InvoiceId { get; }

    public decimal? AmountPaid { get; }
}
