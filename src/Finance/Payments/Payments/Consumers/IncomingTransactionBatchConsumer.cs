using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Payments.Domain;
using YourBrand.Transactions.Contracts;

namespace YourBrand.Payments.Consumers;

public class IncomingTransactionBatchConsumer(IPaymentsContext context, YourBrand.Transactions.Client.ITransactionsClient transactionsClient) : IConsumer<IncomingTransactionBatch>
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
            await transactionsClient.SetTransactionStatusAsync(transaction.OrganizationId, transaction.Id, Transactions.Client.TransactionStatus.Verified);
            return;
        }

        var payment = await context.Payments
            .Include(p => p.Captures)
            .FirstOrDefaultAsync(p => p.Reference == transaction.Reference);

        if (payment is null)
        {
            await transactionsClient.SetTransactionStatusAsync(transaction.OrganizationId, transaction.Id, Transactions.Client.TransactionStatus.Unknown);
        }
        else
        {
            payment.RegisterCapture(transaction.Date, transaction.Amount, transaction.Id);

            var amountCaptured = payment.AmountCaptured;

            if (amountCaptured == payment.Amount)
            {
                payment.SetStatus(Domain.Enums.PaymentStatus.Captured);
            }
            else if (amountCaptured < payment.Amount)
            {
                payment.SetStatus(Domain.Enums.PaymentStatus.PartiallyCaptured);
            }
            else if (amountCaptured > payment.Amount)
            {
                payment.SetStatus(Domain.Enums.PaymentStatus.PartiallyRefunded);
            }

            await context.SaveChangesAsync(cancellationToken);

            await transactionsClient.SetTransactionStatusAsync(transaction.OrganizationId, transaction.Id, Transactions.Client.TransactionStatus.Verified);
        }
    }
}