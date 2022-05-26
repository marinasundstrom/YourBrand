using Transactions.Application.Common.Models;
using Transactions.Domain;
using Transactions.Domain.Events;

using MediatR;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Transactions.Application.Events;

public class TransactionReferenceUpdatedHandler : INotificationHandler<DomainEventNotification<TransactionReferenceUpdated>>
{
    private readonly ITransactionsContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public TransactionReferenceUpdatedHandler(ITransactionsContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(DomainEventNotification<TransactionReferenceUpdated> notification, CancellationToken cancellationToken)
    {
        var t = await _context
            .Transactions
            .FirstOrDefaultAsync(i => i.Id == notification.DomainEvent.TransactionId);

        if(t is not null) 
        {
            await _publishEndpoint.Publish(
                new Contracts.TransactionBatch(new [] { new Contracts.Transaction(t.Id, t.Date, (Contracts.TransactionStatus)t.Status, t.From!, t.Reference!, t.Currency, t.Amount) }));
        }
    }
}