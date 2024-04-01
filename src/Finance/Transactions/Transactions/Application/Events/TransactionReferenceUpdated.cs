using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Transactions.Application.Common.Interfaces;
using YourBrand.Transactions.Domain;
using YourBrand.Transactions.Domain.Events;

namespace YourBrand.Transactions.Application.Events;

public class TransactionReferenceUpdatedHandler : IDomainEventHandler<TransactionReferenceUpdated>
{
    private readonly ITransactionsContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public TransactionReferenceUpdatedHandler(ITransactionsContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(TransactionReferenceUpdated notification, CancellationToken cancellationToken)
    {
        var t = await _context
            .Transactions
            .FirstOrDefaultAsync(i => i.Id == notification.TransactionId);

        if (t is not null)
        {
            await _publishEndpoint.Publish(
                new Contracts.IncomingTransactionBatch(new[] { new Contracts.Transaction(t.Id, t.Date, (Contracts.TransactionStatus)t.Status, t.From!, t.Reference!, t.Currency, t.Amount) }));
        }
    }
}