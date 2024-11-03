using Microsoft.EntityFrameworkCore;

using YourBrand.Domain;
using YourBrand.Transactions.Application.Common.Interfaces;
using YourBrand.Transactions.Domain;
using YourBrand.Transactions.Domain.Events;
using YourBrand.Transactions.Hubs;

namespace YourBrand.Transactions.Application.Events;

public class TransactionStatusChangedHandler(ITransactionsContext context, ITransactionsHubClient transactionsHubClient) : IDomainEventHandler<TransactionStatusChanged>
{
    public async Task Handle(TransactionStatusChanged notification, CancellationToken cancellationToken)
    {
        var transaction = await context
            .Transactions
            .FirstOrDefaultAsync(i => i.Id == notification.TransactionId);

        if (transaction is not null)
        {
            await transactionsHubClient.TransactionStatusUpdated(transaction.Id, transaction.Status);
        }
    }
}