using YourBrand.Transactions.Domain.Enums;

namespace YourBrand.Transactions.Application;

public record PostTransactionDto(string Id, DateTime? Date, TransactionStatus Status, string From, string Reference, string Currency, decimal Amount);

public record TransactionDto(string OrganizationId, string Id, DateTime? Date, TransactionStatus Status, string From, string Reference, string Currency, decimal Amount);