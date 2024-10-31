using YourBrand.Accounting.Client;

using YourBrand.Invoicing.Client;

namespace YourBrand.Accountant.Services;

public class RefundService(IInvoicesClient invoicesClient, IJournalEntriesClient journalEntriesClient, ILogger<RefundService> logger) : IRefundService
{
    public async Task CheckForRefund()
    {
        string organizationId = "";

        logger.LogInformation("Querying for invoices");

        var results = await invoicesClient.GetInvoicesAsync(organizationId, 0, 100, null, new[] { (int)InvoiceStatuses.Overpaid }, null, null, null);

        foreach (var invoice in results.Items)
        {
            if (invoice.Status.Id == (int)InvoiceStatuses.Overpaid)
            {
                logger.LogDebug($"Handling overpaid invoice {invoice.Id}");

                // TODO: Fix calculations with regards to VAT

                var amountToRefund = invoice.Paid - invoice.Total;
                var subTotal = amountToRefund / (1m + 0.25m);
                var vat = amountToRefund - subTotal;

                var journalEntryId = await journalEntriesClient.CreateJournalEntryAsync(organizationId, new CreateJournalEntry
                {
                    Description = $"Betalade tillbaka för överbetalad faktura #{invoice.InvoiceNo}",
                    InvoiceNo = invoice.InvoiceNo,
                    Entries = new[]
                    {
                        new CreateEntry
                        {
                            AccountNo = 1510,
                            Description = string.Empty,
                            Credit = amountToRefund
                        },
                        new CreateEntry
                        {
                            AccountNo = 2610,
                            Description = string.Empty,
                            Debit = vat
                        },
                        new CreateEntry
                        {
                            AccountNo = 3001,
                            Description = string.Empty,
                            Debit = amountToRefund - vat
                        }
                    }
                });

                await invoicesClient.SetInvoiceStatusAsync(organizationId, invoice.Id, (int)InvoiceStatuses.PartiallyRepaid);
            }
        }
    }
}