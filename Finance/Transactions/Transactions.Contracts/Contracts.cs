namespace Transactions.Contracts;

public record TransactionBatch(IEnumerable<Transaction> Transactions);

public record Transaction(string Id, DateTime Date, TransactionStatus Status, string From, string Reference, string Currency, decimal Amount);

public enum TransactionStatus
{
    Unverified,
    Verified,
    Payback,
    Unknown,
}