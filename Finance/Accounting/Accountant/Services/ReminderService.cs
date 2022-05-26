using System.Text;

using Accounting.Client;

using Documents.Client;

using Invoices.Client;

using Newtonsoft.Json;

namespace Accountant.Services
{
    public class ReminderService : IReminderService
    {
        private readonly IInvoicesClient _invoicesClient;
        private readonly IVerificationsClient _verificationsClient;
        private readonly IDocumentsClient _documentsClient;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<RefundService> _logger;

        public ReminderService(IInvoicesClient invoicesClient, IVerificationsClient verificationsClient,
            IDocumentsClient documentsClient, IServiceScopeFactory serviceScopeFactory, ILogger<RefundService> logger)
        {
            _invoicesClient = invoicesClient;
            _verificationsClient = verificationsClient;
            _documentsClient = documentsClient;
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        public async Task IssueReminders()
        {
            _logger.LogInformation("Querying for invoices");

            var results = await _invoicesClient.GetInvoicesAsync(0, 100, null, new[] { InvoiceStatus.PartiallyPaid, InvoiceStatus.Sent }, null);

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                foreach (var invoice in results.Items)
                {
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                    if (invoice.Status == InvoiceStatus.PartiallyPaid)
                    {
                        _logger.LogDebug($"Notify customer about partially paid invoice {invoice.Id}");

                        // Send email

                        var model = JsonConvert.SerializeObject(
                                JsonConvert.SerializeObject(new { 
                                    Name = "Test1", 
                                    RemainingAmount = (invoice.Total - invoice.Paid) 
                                }));

                        string templateId = "reminder2";

                        string text = await GenerateDocument(model, templateId);

                        await emailService.SendEmail("test1@foo.com", "You have partially paid", text);
                    }
                    else if (invoice.Status == InvoiceStatus.Sent || invoice.Status == InvoiceStatus.Reminder)
                    {   
                        if (DateTime.Now > invoice.DueDate)
                        {
                            _logger.LogDebug($"Notify customer about forgotten invoice {invoice.Id}");

                            // Send email

                            var model = JsonConvert.SerializeObject(
                                JsonConvert.SerializeObject(new { 
                                    Name = "Test2" ,
                                    AmountToPay = invoice.Total
                                }));

                            string templateId = "reminder";

                            await _invoicesClient.SetInvoiceStatusAsync(invoice.Id, InvoiceStatus.Reminder);

                            string text = await GenerateDocument(model, templateId);

                            await emailService.SendEmail("test2@foo.com", "You have not paid your invoice", text);
                        }
                    }
                }
            }
        }

        private async Task<string> GenerateDocument(string model, string templateId)
        {
            var response = await _documentsClient.GenerateDocumentAsync(templateId, DocumentFormat.Html, model);

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
}

