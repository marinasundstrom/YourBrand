using MassTransit;

using YourBrand.Accounting.Client;
using YourBrand.Invoicing.Client;
using YourBrand.Payments.Client;
using YourBrand.Payments.Contracts;

namespace YourBrand.Accountant.Consumers;

public class PaymentCapturedConsumer(IJournalEntriesClient journalEntriesClient, IInvoicesClient invoicesClient, IPaymentsClient paymentsClient) : IConsumer<PaymentCaptured>
{
    public async Task Consume(ConsumeContext<PaymentCaptured> context)
    {
        string organizationId = "";
        
        CancellationToken cancellationToken = context.CancellationToken;

        var capture = context.Message;

        // TODO: Add Reference to PaymentCapture
        PaymentDto payment = await paymentsClient.GetPaymentByIdAsync(capture.PaymentId);

        // Find invoice
        // Check invoice status
        // Is it underpaid?
        // Overpaid? Pay back

        // Create verification

        var invoice = await invoicesClient.GetInvoiceAsync(organizationId, payment.InvoiceId, cancellationToken);

        var capturedAmount = capture.Amount;

        switch (invoice.Status.Id)
        {
            case (int)InvoiceStatuses.Draft:
                // Do nothing
                return;

            case (int)InvoiceStatuses.Sent:
            case (int)InvoiceStatuses.Reminder:
                if (capturedAmount < invoice.Total)
                {
                    await invoicesClient.SetInvoiceStatusAsync(organizationId, invoice.Id, (int)InvoiceStatuses.PartiallyPaid, cancellationToken);
                }
                else if (capturedAmount == invoice.Total)
                {
                    await invoicesClient.SetInvoiceStatusAsync(organizationId, invoice.Id, (int)InvoiceStatuses.Paid, cancellationToken);
                }
                else if (capturedAmount > invoice.Total)
                {
                    await invoicesClient.SetInvoiceStatusAsync(organizationId, invoice.Id, (int)InvoiceStatuses.Overpaid, cancellationToken);
                }
                await invoicesClient.SetPaidAmountAsync(organizationId, invoice.Id, invoice.Paid.GetValueOrDefault() + capturedAmount);
                break;

            case (int)InvoiceStatuses.Paid:
            case (int)InvoiceStatuses.PartiallyPaid:
            case (int)InvoiceStatuses.Overpaid:
                var paidAmount = invoice.Paid.GetValueOrDefault() + capturedAmount;
                if (paidAmount < invoice.Total)
                {
                    await invoicesClient.SetInvoiceStatusAsync(organizationId, invoice.Id, (int)InvoiceStatuses.PartiallyPaid, cancellationToken);
                }
                else if (paidAmount == invoice.Total)
                {
                    await invoicesClient.SetInvoiceStatusAsync(organizationId, invoice.Id, (int)InvoiceStatuses.Paid, cancellationToken);
                }
                else if (paidAmount > invoice.Total)
                {
                    await invoicesClient.SetInvoiceStatusAsync(organizationId, invoice.Id, (int)InvoiceStatuses.Overpaid, cancellationToken);
                }
                await invoicesClient.SetPaidAmountAsync(organizationId, invoice.Id, paidAmount);
                break;

            case (int)InvoiceStatuses.Void:
                // Mark transaktion for re-pay
                //await _transactionsClient.SetTransactionStatusAsync(payment.Id, YourBrand.Transactions.Client.TransactionStatus.Payback);

                // Create verification in a worker
                return;
        }

        //await _transactionsClient.SetTransactionInvoiceIdAsync(payment.Id, invoiceId);

        //await _transactionsClient.SetTransactionStatusAsync(payment.Id, YourBrand.Transactions.Client.TransactionStatus.Verified);

        var entries = new List<CreateEntry>
            {
                    new CreateEntry
                    {
                        AccountNo = 1920,
                        Description = string.Empty,
                        Debit = capturedAmount
                    },
                    new CreateEntry
                    {
                        AccountNo = 1510, // Can be other
                        Description = string.Empty,
                        Credit = capturedAmount
                    }
                };

        var journalEntryId = await journalEntriesClient.CreateJournalEntryAsync(organizationId, new CreateJournalEntry
        {
            Description = $"Betalade faktura #{invoice.InvoiceNo}",
            InvoiceNo = invoice.InvoiceNo,
            Entries = entries
        }, cancellationToken);

        //await _journalEntriesClient.AddFileAttachmentToVerificationAsync(verificationId, new FileParameter());
    }
}