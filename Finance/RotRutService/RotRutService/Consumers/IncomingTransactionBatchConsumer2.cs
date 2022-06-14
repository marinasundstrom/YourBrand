using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Accounting.Client;
using YourBrand.RotRutService.Domain;
using YourBrand.RotRutService.Domain.Entities;
using YourBrand.Transactions.Client;
using YourBrand.Transactions.Contracts;

namespace YourBrand.RotRutService.Consumers;

public class IncomingTransactionBatchConsumer2 : IConsumer<IncomingTransactionBatch>
{
    private readonly IRotRutContext _context;
    private readonly IVerificationsClient _verificationsClient;
    private readonly ITransactionsClient _transactionsClient;

    public IncomingTransactionBatchConsumer2(IRotRutContext context, IVerificationsClient verificationsClient, ITransactionsClient transactionsClient)
    {
        _context = context;
        _verificationsClient = verificationsClient;
        _transactionsClient = transactionsClient;
    }

    public async Task Consume(ConsumeContext<IncomingTransactionBatch> context)
    {
        var batch = context.Message;

        foreach (var transaction in batch.Transactions)
        {
            await HandleTransaction(transaction, context.CancellationToken);
        }
    }

    private async Task HandleTransaction(Transaction transaction, CancellationToken cancellationToken)
    {
        if (transaction.Reference == "Skatteverket")
        {
            Console.WriteLine("Skatteverket!");
        }
    }
}