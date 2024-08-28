using MassTransit;

using YourBrand.Accounting.Client;
using YourBrand.RotRutService.Domain;
using YourBrand.Transactions.Contracts;

namespace YourBrand.RotRutService.Consumers;

public class IncomingTransactionBatchConsumer2(IRotRutContext context, IJournalEntriesClient verificationsClient, YourBrand.Transactions.Client.ITransactionsClient transactionsClient) : IConsumer<IncomingTransactionBatch>
{
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