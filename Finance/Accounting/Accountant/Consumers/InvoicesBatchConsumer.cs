using Accounting.Client;

using Documents.Client;

using Invoices.Client;
using Invoices.Contracts;

using MassTransit;

namespace Accountant.Consumers;

public class InvoicesBatchConsumer : IConsumer<InvoicesBatch>
{
    private readonly IVerificationsClient _verificationsClient;
    private readonly IInvoicesClient _invoicesClient;
    private readonly IDocumentsClient _documentsClient;

    public InvoicesBatchConsumer(IVerificationsClient verificationsClient, 
        IInvoicesClient invoicesClient, IDocumentsClient documentsClient)
    {
        _verificationsClient = verificationsClient;
        _invoicesClient = invoicesClient;
        _documentsClient = documentsClient;
    }

    public async Task Consume(ConsumeContext<InvoicesBatch> context)
    {
        var batch = context.Message;

        foreach (var invoice in batch.Invoices)
        {
            await HandleInvoice(invoice, context.CancellationToken);
        }
    }

    private async Task HandleInvoice(Invoice i, CancellationToken cancellationToken)
    {
        // Get invoice
        // Register entries

        var invoice = await _invoicesClient.GetInvoiceAsync(i.Id, cancellationToken);

        if (invoice.Status != InvoiceStatus.Sent)
        {
            return;
        }

        var itemsGroupedByTypeAndVatRate = invoice.Items.GroupBy(item => new { item.ProductType, item.VatRate });

        List<CreateEntry> entries = new();

        foreach (var group in itemsGroupedByTypeAndVatRate)
        {
            var productType = group.Key.ProductType;
            var vatRate = group.Key.VatRate;

            var vat = group.Sum(i => i.LineTotal).Vat(vatRate);
            var subTotal = group.Sum(i => i.LineTotal).SubTotal(vatRate);

            entries.Add(new CreateEntry
            {
                AccountNo = 1510,
                Description = string.Empty,
                Debit = invoice.Total
            });

            if (productType == ProductType.Good)
            {
                if (vatRate == 0.25)
                {
                    entries.AddRange(new[] {
                        new CreateEntry
                        {
                            AccountNo = 2610,
                            Description = string.Empty,
                            Credit = vat
                        },
                        new CreateEntry
                        {
                            AccountNo = 3001,
                            Description = string.Empty,
                            Credit = subTotal
                        }
                    });
                }
                else if (vatRate == 0.12)
                {
                    entries.AddRange(new[] {
                        new CreateEntry
                        {
                            AccountNo = 2610,
                            Description = string.Empty,
                            Credit = vat
                        },
                        new CreateEntry
                        {
                            AccountNo = 3002,
                            Description = string.Empty,
                            Credit = subTotal
                        }
                    });
                }
                else if (vatRate == 0.06)
                {
                    entries.AddRange(new[] {
                        new CreateEntry
                        {
                            AccountNo = 2610,
                            Description = string.Empty,
                            Credit = vat
                        },
                        new CreateEntry
                        {
                            AccountNo = 3003,
                            Description = string.Empty,
                            Credit = subTotal
                        }
                    });
                }
            }
            else if (productType == ProductType.Service)
            {
                if (vatRate == 0.25)
                {
                    entries.AddRange(new[] {
                        new CreateEntry
                        {
                            AccountNo = 2610,
                            Description = string.Empty,
                            Credit = vat
                        },
                        new CreateEntry
                        {
                            AccountNo = 3041,
                            Description = string.Empty,
                            Credit = subTotal
                        }
                    });
                }
            }
        }

        var verificationId = await _verificationsClient.CreateVerificationAsync(new CreateVerification
        {
            Description = $"Skickade ut faktura #{i.Id}",
            InvoiceId = invoice.Id,
            Entries = entries
        }, cancellationToken);

        var file = await _invoicesClient.GetInvoiceFileAsync(invoice.Id);

        string filename = GetFileName(file);

        var fileExt = Path.GetExtension(filename);

        string contentType = GetContentType(file);

        await _verificationsClient.AddFileAttachmentToVerificationAsync(
            verificationId, null, invoice.Id,
            new Accounting.Client.FileParameter(file.Stream, $"invoice-{invoice.Id}{fileExt}", contentType));
    }

    private static string? GetContentType(Invoices.Client.FileResponse file)
    {
        var contentType = file.Headers
            .FirstOrDefault(x => x.Key.ToLowerInvariant() == "content-type").Value;

        if(contentType is null)
            return null;

        return contentType.FirstOrDefault();
    }

    private static string? GetFileName(Invoices.Client.FileResponse file)
    {
        var contentDisposition = file.Headers
            .FirstOrDefault(x => x.Key.ToLowerInvariant() == "content-disposition").Value;

        if(contentDisposition is null)
            return null;

        var value = contentDisposition.FirstOrDefault();

         if(value is null)
            return null;

        var filenamePart = value.Split(';').FirstOrDefault(x => x.Contains("filename="));

        if(filenamePart is null)
            return null;

        var fileName = filenamePart.Substring(filenamePart.IndexOf("=") + 1);

        /*
        var filenamePart = file.Headers
            .First(x => x.Key.ToLowerInvariant() == "content-disposition").Value
            .First().Split(';').First(x => x.Contains("filename="));

        var filename = filenamePart.Substring(filenamePart.IndexOf("=") + 1);
        */

        return fileName;
    }
}