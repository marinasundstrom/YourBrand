using Transactions.Domain.Common;

namespace Transactions.Domain.Events;

public class TransactionRegistered : DomainEvent
{
    public TransactionRegistered(string transactionId)
    {
        TransactionId = transactionId;
    }

    public string TransactionId { get; }
}