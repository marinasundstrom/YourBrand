using YourBrand.Domain;
using YourBrand.Transactions.Domain.Common;

namespace YourBrand.Transactions.Domain.Events;

public record TransactionRegistered : DomainEvent
{
    public TransactionRegistered(string transactionId)
    {
        TransactionId = transactionId;
    }

    public string TransactionId { get; }
}