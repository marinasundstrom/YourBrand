using Transactions.Domain.Enums;

namespace Transactions.Application;

public record TransactionDto(string Id, DateTime? Date, TransactionStatus Status, string From, string Reference, string Currency, decimal Amount, int? InvoiceId);