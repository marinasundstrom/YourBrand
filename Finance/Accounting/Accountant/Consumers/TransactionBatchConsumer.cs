using Accounting.Client;

using Invoices.Client;

using MassTransit;

using Transactions.Client;
using Transactions.Contracts;

namespace Accountant.Consumers;

public class TransactionBatchConsumer : IConsumer<TransactionBatch>
{
    private readonly IVerificationsClient _verificationsClient;
    private readonly IInvoicesClient _invoicesClient;
    private readonly ITransactionsClient _transactionsClient;

    public TransactionBatchConsumer(IVerificationsClient verificationsClient, IInvoicesClient invoicesClient, ITransactionsClient transactionsClient)
    {
        _verificationsClient = verificationsClient;
        _invoicesClient = invoicesClient;
        _transactionsClient = transactionsClient;
    }

    public async Task Consume(ConsumeContext<TransactionBatch> context)
    {
        var batch = context.Message;

        foreach (var transaction in batch.Transactions)
        {
            await HandleTransaction(transaction, context.CancellationToken);
        }
    }

    private async Task HandleTransaction(Transaction transaction, CancellationToken cancellationToken)
    {
        // Find invoice
        // Check invoice status
        // Is it underpaid?
        // Overpaid? Pay back

        // Create verification

        // Not found set transaction status unknown

        if (!int.TryParse(transaction.Reference, out var invoiceId))
        {
            // Mark transaction as unknown
            await _transactionsClient.SetTransactionStatusAsync(transaction.Id, Transactions.Client.TransactionStatus.Unknown);
            return;
        }

        var invoice = await _invoicesClient.GetInvoiceAsync(invoiceId, cancellationToken);

        if (invoice is null)
        {
            // Mark transaction as unknown
            await _transactionsClient.SetTransactionStatusAsync(transaction.Id, Transactions.Client.TransactionStatus.Unknown);
        }
        else
        {
            var receivedAmount = transaction.Amount;

            switch (invoice.Status)
            {
                case InvoiceStatus.Draft:
                    await _transactionsClient.SetTransactionStatusAsync(transaction.Id, Transactions.Client.TransactionStatus.Unknown);
                    return;

                case InvoiceStatus.Sent:
                case InvoiceStatus.Reminder:
                    if (receivedAmount < invoice.Total)
                    {
                        await _invoicesClient.SetInvoiceStatusAsync(invoice.Id, InvoiceStatus.PartiallyPaid, cancellationToken);
                    }
                    else if (receivedAmount == invoice.Total)
                    {
                        await _invoicesClient.SetInvoiceStatusAsync(invoice.Id, InvoiceStatus.Paid, cancellationToken);
                    }
                    else if (receivedAmount > invoice.Total)
                    {
                        await _invoicesClient.SetInvoiceStatusAsync(invoice.Id, InvoiceStatus.Overpaid, cancellationToken);
                    }
                    await _invoicesClient.SetPaidAmountAsync(invoice.Id, invoice.Paid.GetValueOrDefault() + receivedAmount);
                    break;

                case InvoiceStatus.Paid:
                case InvoiceStatus.PartiallyPaid:
                case InvoiceStatus.Overpaid:
                    var paidAmount = invoice.Paid.GetValueOrDefault() + receivedAmount;
                    if (paidAmount < invoice.Total)
                    {
                        await _invoicesClient.SetInvoiceStatusAsync(invoice.Id, InvoiceStatus.PartiallyPaid, cancellationToken);
                    }
                    else if (paidAmount == invoice.Total)
                    {
                        await _invoicesClient.SetInvoiceStatusAsync(invoice.Id, InvoiceStatus.Paid, cancellationToken);
                    }
                    else if (paidAmount > invoice.Total) 
                    {
                        await _invoicesClient.SetInvoiceStatusAsync(invoice.Id, InvoiceStatus.Overpaid, cancellationToken);
                    }
                    await _invoicesClient.SetPaidAmountAsync(invoice.Id, paidAmount);
                    break;

                case InvoiceStatus.Void:
                    // Mark transaktion for re-pay
                    await _transactionsClient.SetTransactionStatusAsync(transaction.Id, Transactions.Client.TransactionStatus.Payback);

                    // Create verification in a worker
                    return;
            }

            await _transactionsClient.SetTransactionInvoiceIdAsync(transaction.Id, invoiceId);

            await _transactionsClient.SetTransactionStatusAsync(transaction.Id, Transactions.Client.TransactionStatus.Verified);

            var verificationId = await _verificationsClient.CreateVerificationAsync(new CreateVerification
            {
                Description = $"Betalade faktura #{invoice.Id}",
                InvoiceId = invoice.Id,
                Entries = new[]
                {
                    new CreateEntry
                    {
                        AccountNo = 1920,
                        Description = string.Empty,
                        Debit = transaction.Amount
                    },
                    new CreateEntry
                    {
                        AccountNo = 1510,
                        Description = string.Empty,
                        Credit = transaction.Amount
                    }
                }
            }, cancellationToken);

            //await _verificationsClient.AddFileAttachmentToVerificationAsync(verificationId, new FileParameter());
        }
    }
}