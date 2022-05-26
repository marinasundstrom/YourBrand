using YourBrand.Transactions.Domain.Common;

namespace YourBrand.Transactions.Domain.Events;

public class TransactionInvoiceIdUpdated : DomainEvent
{
    public TransactionInvoiceIdUpdated(string transactionId, int invoiceId)
    {
        TransactionId = transactionId;
        InvoiceId = invoiceId;
    }

    public string TransactionId { get; }

    public int InvoiceId { get; }
}
