using YourBrand.Transactions.Domain.Common;
using YourBrand.Transactions.Domain.Enums;

namespace YourBrand.Transactions.Domain.Events;

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