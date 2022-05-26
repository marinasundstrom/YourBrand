using Transactions.Domain.Common;

namespace Transactions.Domain.Events;

public class TransactionReferenceUpdated : DomainEvent
{
    public TransactionReferenceUpdated(string transactionId, string reference)
    {
        TransactionId = transactionId;
        Reference = reference;
    }

    public string TransactionId { get; }

    public string Reference { get; }
}