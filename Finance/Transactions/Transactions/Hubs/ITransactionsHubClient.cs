using YourBrand.Transactions.Domain.Enums;

namespace YourBrand.Transactions.Hubs;

public interface ITransactionsHubClient
{
    Task TransactionStatusUpdated(string id, TransactionStatus Status);

    Task TransactionInvoiceIdUpdated(string id, int? invoiceId);
}