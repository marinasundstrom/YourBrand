using Transactions.Domain.Enums;

namespace Transactions.Hubs;

public interface ITransactionsHubClient
{
    Task TransactionStatusUpdated(string id, TransactionStatus Status);

    Task TransactionInvoiceIdUpdated(string id, int? invoiceId);
}