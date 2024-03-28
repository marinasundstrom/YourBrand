using YourBrand.Accounting.Client;

using YourBrand.Invoicing.Client;

using MassTransit;

using YourBrand.Payments.Contracts;
using YourBrand.Payments.Client;

namespace YourBrand.Accountant.Consumers;

public class PaymentCapturedConsumer : IConsumer<PaymentCaptured>
{
    private readonly IJournalEntriesClient _verificationsClient;
    private readonly IInvoicesClient _invoicesClient;
    private readonly IPaymentsClient _paymentsClient;

    public PaymentCapturedConsumer(IJournalEntriesClient verificationsClient, IInvoicesClient invoicesClient, IPaymentsClient paymentsClient)
    {
        _verificationsClient = verificationsClient;
        _invoicesClient = invoicesClient;
        _paymentsClient = paymentsClient;
    }

    public async Task Consume(ConsumeContext<PaymentCaptured> context)
    {
        CancellationToken cancellationToken = context.CancellationToken;

        var capture = context.Message;

        // TODO: Add Reference to PaymentCapture
        PaymentDto payment = await _paymentsClient.GetPaymentByIdAsync(capture.PaymentId);

        // Find invoice
        // Check invoice status
        // Is it underpaid?
        // Overpaid? Pay back

        // Create verification

        var invoice = await _invoicesClient.GetInvoiceAsync(payment.InvoiceId, cancellationToken);

        var capturedAmount = capture.Amount;

        switch (invoice.Status)
        {
            case InvoiceStatus.Draft:
                // Do nothing
                return;

            case InvoiceStatus.Sent:
            case InvoiceStatus.Reminder:
                if (capturedAmount < invoice.Total)
                {
                    await _invoicesClient.SetInvoiceStatusAsync(invoice.Id, InvoiceStatus.PartiallyPaid, cancellationToken);
                }
                else if (capturedAmount == invoice.Total)
                {
                    await _invoicesClient.SetInvoiceStatusAsync(invoice.Id, InvoiceStatus.Paid, cancellationToken);
                }
                else if (capturedAmount > invoice.Total)
                {
                    await _invoicesClient.SetInvoiceStatusAsync(invoice.Id, InvoiceStatus.Overpaid, cancellationToken);
                }
                await _invoicesClient.SetPaidAmountAsync(invoice.Id, invoice.Paid.GetValueOrDefault() + capturedAmount);
                break;

            case InvoiceStatus.Paid:
            case InvoiceStatus.PartiallyPaid:
            case InvoiceStatus.Overpaid:
                var paidAmount = invoice.Paid.GetValueOrDefault() + capturedAmount;
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

        var journalEntryId = await _verificationsClient.CreateJournalEntryAsync(new CreateJournalEntry
        {
            Description = $"Betalade faktura #{invoice.InvoiceNo}",
            InvoiceNo = int.Parse(invoice.InvoiceNo),
            Entries = entries
        }, cancellationToken);

        //await _verificationsClient.AddFileAttachmentToVerificationAsync(verificationId, new FileParameter());
    }
}