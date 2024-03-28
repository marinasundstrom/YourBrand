namespace YourBrand.Transactions.Contracts;

public record IncomingTransactionBatch(IEnumerable<Transaction> Transactions);

public record Transaction(string Id, DateTime Date, TransactionStatus Status, string From, string Reference, string Currency, decimal Amount);

public enum TransactionStatus
{
    Unverified,
    Verified,
    Payback,
    Unknown,
}