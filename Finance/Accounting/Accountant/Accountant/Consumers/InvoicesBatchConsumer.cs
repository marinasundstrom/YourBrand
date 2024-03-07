using YourBrand.Accounting.Client;

using YourBrand.Documents.Client;

using YourBrand.Invoicing.Client;
using YourBrand.Invoicing.Contracts;

using MassTransit;
using YourBrand.Accountant.Domain;

namespace YourBrand.Accountant.Consumers;

public class InvoicesBatchConsumer : IConsumer<InvoicesBatch>
{
    private readonly IVerificationsClient _verificationsClient;
    private readonly IInvoicesClient _invoicesClient;
    private readonly IDocumentsClient _documentsClient;
    private readonly EntriesFactory _cls;

    public InvoicesBatchConsumer(IVerificationsClient verificationsClient,
        IInvoicesClient invoicesClient, IDocumentsClient documentsClient, EntriesFactory cls)
    {
        _verificationsClient = verificationsClient;
        _invoicesClient = invoicesClient;
        _documentsClient = documentsClient;
        _cls = cls;
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

        var entries = _cls.GetEntries(invoice);

        var verificationId = await _verificationsClient.CreateVerificationAsync(new CreateVerification
        {
            Description = $"Skickade ut faktura #{i.Id}",
            InvoiceId = int.Parse(invoice.Id),
            Entries = entries.ToList(),
        }, cancellationToken);

        var file = await _invoicesClient.GetInvoiceFileAsync(invoice.Id);

        string filename = GetFileName(file);

        var fileExt = Path.GetExtension(filename);

        string contentType = GetContentType(file);

        using var stream = new MemoryStream();
        file.Stream.CopyTo(stream);

        stream.Seek(0, SeekOrigin.Begin);

        using var stream2 = new MemoryStream();
        stream.CopyTo(stream2);

        stream2.Seek(0, SeekOrigin.Begin);
        stream.Seek(0, SeekOrigin.Begin);

        await _verificationsClient.AddFileAttachmentToVerificationAsync(
            verificationId, null, int.Parse(invoice.Id),
            new Accounting.Client.FileParameter(stream, $"invoice-{invoice.Id}{fileExt}", contentType));

        try 
        {            
            await _documentsClient.UploadDocumentAsync("invoices",
                new Documents.Client.FileParameter(stream2, $"invoice-{invoice.Id}{fileExt}", contentType));
        }
        catch(Exception exc) 
        {
            Console.WriteLine($"Failed to upload document: {exc}");
        }
    }

    private static string? GetContentType(Invoicing.Client.FileResponse file)
    {
        var contentType = file.Headers
            .FirstOrDefault(x => x.Key.ToLowerInvariant() == "content-type").Value;

        if (contentType is null)
            return null;

        return contentType.FirstOrDefault();
    }

    private static string? GetFileName(Invoicing.Client.FileResponse file)
    {
        var contentDisposition = file.Headers
            .FirstOrDefault(x => x.Key.ToLowerInvariant() == "content-disposition").Value;

        if (contentDisposition is null)
            return null;

        var value = contentDisposition.FirstOrDefault();

        if (value is null)
            return null;

        var filenamePart = value.Split(';').FirstOrDefault(x => x.Contains("filename="));

        if (filenamePart is null)
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