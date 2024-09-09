using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Transactions.Application.Common.Interfaces;
using YourBrand.Transactions.Domain;
using YourBrand.Transactions.Domain.Events;

namespace YourBrand.Transactions.Application.Events;

public class TransactionReferenceUpdatedHandler(ITransactionsContext context, IPublishEndpoint publishEndpoint) : IDomainEventHandler<TransactionReferenceUpdated>
{
    public async Task Handle(TransactionReferenceUpdated notification, CancellationToken cancellationToken)
    {
        var t = await context
            .Transactions
            .InOrganization(notification.OrganizationId)
            .FirstOrDefaultAsync(i => i.Id == notification.TransactionId);

        if (t is not null)
        {
            await publishEndpoint.Publish(
                new Contracts.IncomingTransactionBatch(new[] { new Contracts.Transaction(t.Id, t.OrganizationId, t.Date, (Contracts.TransactionStatus)t.Status, t.From!, t.Reference!, t.Currency, t.Amount) }));
        }
    }
}