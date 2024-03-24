using YourBrand.Accounting.Client;

using YourBrand.Documents.Client;

using YourBrand.Invoicing.Client;
using YourBrand.Invoicing.Contracts;

using MassTransit;
using YourBrand.Accountant.Domain;

namespace YourBrand.Accountant.Consumers;

public class InvoicesBatchConsumer : IConsumer<InvoicesBatch>
{
    private readonly IJournalEntriesClient _journalEntriesClient;
    private readonly IInvoicesClient _invoicesClient;
    private readonly IDocumentsClient _documentsClient;
    private readonly EntriesFactory _entriesFactory;
    private readonly ILogger<InvoicesBatchConsumer> _logger;

    public InvoicesBatchConsumer(IJournalEntriesClient verificationsClient,
        IInvoicesClient invoicesClient, IDocumentsClient documentsClient, EntriesFactory entriesFactory,
        ILogger<InvoicesBatchConsumer> logger)
    {
        _journalEntriesClient = verificationsClient;
        _invoicesClient = invoicesClient;
        _documentsClient = documentsClient;
        _entriesFactory = entriesFactory;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<InvoicesBatch> context)
    {
        var batch = context.Message;

        foreach (var invoice in batch.Invoices)
        {
            await HandleInvoice(invoice, context.CancellationToken);
        }

        //await Task.WhenAll(batch.Invoices.Select(x => HandleInvoice(x, context.CancellationToken)));
    }

    private async Task HandleInvoice(YourBrand.Invoicing.Contracts.Invoice i, CancellationToken cancellationToken)
    {
        // Get invoice
        // Register entries

        var invoice = await _invoicesClient.GetInvoiceAsync(i.Id, cancellationToken);

        if (invoice.Status != InvoiceStatus.Sent)
        {
            return;
        }

        await CreateVerificationFromInvoice(invoice, cancellationToken);
    }

    private async Task CreateVerificationFromInvoice(YourBrand.Invoicing.Client.Invoice invoice, CancellationToken cancellationToken)
    {
        var entries = _entriesFactory.CreateEntriesFromInvoice(invoice);

        var journalEntryId = await _journalEntriesClient.CreateJournalEntryAsync(new CreateJournalEntry
        {
            Description = $"Skickade ut faktura #{invoice.InvoiceNo}",
            InvoiceNo = int.Parse(invoice.InvoiceNo),
            Entries = entries.ToList(),
        }, cancellationToken);

        try 
        {
            await UploadDocuments(invoice, journalEntryId);
        }
        catch(Exception e) 
        {
            _logger.LogError(e, "Failed to add verification to journal entry,");
        }
    }

    private async Task UploadDocuments(YourBrand.Invoicing.Client.Invoice invoice, int journalEntryId)
    {
        MemoryStream stream, stream2;

        var file = await _invoicesClient.GetInvoiceFileAsync(invoice.Id);

        string filename = GetFileName(file);

        var fileExt = Path.GetExtension(filename);

        string contentType = GetContentType(file);
        stream = new MemoryStream();
        file.Stream.CopyTo(stream);

        stream.Seek(0, SeekOrigin.Begin);
        stream2 = new MemoryStream();
        stream.CopyTo(stream2);

        stream2.Seek(0, SeekOrigin.Begin);
        stream.Seek(0, SeekOrigin.Begin);

        await _journalEntriesClient.AddFileToJournalEntryAsVerificationAsync(
            journalEntryId, null, int.Parse(invoice.Id),
            new Accounting.Client.FileParameter(stream, $"invoice-{invoice.Id}{fileExt}", contentType));

        try
        {
            await _documentsClient.UploadDocumentAsync("invoices",
                new Documents.Client.FileParameter(stream2, $"invoice-{invoice.Id}{fileExt}", contentType));
        }
        catch (Exception exc)
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