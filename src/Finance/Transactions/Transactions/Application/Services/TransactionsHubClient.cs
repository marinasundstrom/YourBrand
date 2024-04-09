using Microsoft.AspNetCore.SignalR;

using YourBrand.Transactions.Domain.Enums;
using YourBrand.Transactions.Hubs;

namespace YourBrand.Transactions.Application.Services;

public class TransactionsHubClient(IHubContext<TransactionsHub, ITransactionsHubClient> hubContext) : ITransactionsHubClient
{
    public async Task TransactionStatusUpdated(string id, TransactionStatus status)
    {
        await hubContext.Clients.All.TransactionStatusUpdated(id, status);
    }

    public async Task TransactionInvoiceIdUpdated(string id, int? invoiceId)
    {
        await hubContext.Clients.All.TransactionInvoiceIdUpdated(id, invoiceId);
    }
}