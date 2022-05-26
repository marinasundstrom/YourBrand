using Transactions.Domain.Common;
using Transactions.Domain.Enums;

namespace Transactions.Domain.Events;

public class TransactionStatusChanged : DomainEvent
{
    public TransactionStatusChanged(string transactionId, TransactionStatus status)
    {
        TransactionId = transactionId;
        Status = status;
    }

    public string TransactionId { get; }

    public TransactionStatus Status { get; }
}