using System.Text;

using Newtonsoft.Json;

using YourBrand.Accounting.Client;
using YourBrand.Documents.Client;
using YourBrand.Invoicing.Client;

namespace YourBrand.Accountant.Services;

public class ReminderService(IInvoicesClient invoicesClient, IJournalEntriesClient journalEntriesClient,
    IDocumentsClient documentsClient, IServiceScopeFactory serviceScopeFactory, ILogger<RefundService> logger) : IReminderService
{
    public async Task IssueReminders()
    {
        string organizationId = "";

        logger.LogInformation("Querying for invoices");

        var results = await invoicesClient.GetInvoicesAsync(organizationId, null, 0, 100, null, [(int)InvoiceStatuses.PartiallyPaid, (int)InvoiceStatuses.Sent], null, null, null);

        using (var scope = serviceScopeFactory.CreateScope())
        {
            foreach (var invoice in results.Items)
            {
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                if (invoice.Status.Id == (int)InvoiceStatuses.PartiallyPaid)
                {
                    logger.LogDebug($"Notify customer about partially paid invoice {invoice.Id}");

                    // Send email

                    var model = JsonConvert.SerializeObject(
                            JsonConvert.SerializeObject(new
                            {
                                Name = "Test1",
                                RemainingAmount = (invoice.Total - invoice.Paid)
                            }));

                    string templateId = "reminder2";

                    string text = await GenerateDocument(model, templateId);

                    await emailService.SendEmail("test1@foo.com", "You have partially paid", text);
                }
                else if (invoice.Status.Id == (int)InvoiceStatuses.Sent || invoice.Status.Id == (int)InvoiceStatuses.Reminder)
                {
                    if (DateTimeOffset.UtcNow > invoice.DueDate)
                    {
                        logger.LogDebug($"Notify customer about forgotten invoice {invoice.Id}");

                        // Send email

                        var model = JsonConvert.SerializeObject(
                            JsonConvert.SerializeObject(new
                            {
                                Name = "Test2",
                                AmountToPay = invoice.Total
                            }));

                        string templateId = "reminder";

                        await invoicesClient.SetInvoiceStatusAsync(organizationId, invoice.Id, (int)InvoiceStatuses.Reminder);

                        string text = await GenerateDocument(model, templateId);

                        await emailService.SendEmail("test2@foo.com", "You have not paid your invoice", text);
                    }
                }
            }
        }
    }

    private async Task<string> GenerateDocument(string model, string templateId)
    {
        var response = await documentsClient.GenerateDocumentAsync(templateId, DocumentFormat.Html, model);

        byte[] bytes;

        using (var memoryStream = new MemoryStream())
        {
            response.Stream.CopyTo(memoryStream);
            bytes = memoryStream.ToArray();
        }

        var text = Encoding.UTF8.GetString(bytes);
        return text;
    }
}