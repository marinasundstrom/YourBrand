using Microsoft.AspNetCore.SignalR;

using Transactions.Domain.Enums;
using Transactions.Hubs;

namespace Transactions.Application.Services;

public class TransactionsHubClient : ITransactionsHubClient
{
    private readonly IHubContext<TransactionsHub, ITransactionsHubClient> _hubContext;

    public TransactionsHubClient(IHubContext<TransactionsHub, ITransactionsHubClient> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task TransactionStatusUpdated(string id, TransactionStatus status)
    {
        await _hubContext.Clients.All.TransactionStatusUpdated(id, status);
    }

    public async Task TransactionInvoiceIdUpdated(string id, int? invoiceId)
    {
        await _hubContext.Clients.All.TransactionInvoiceIdUpdated(id, invoiceId);
    }
}