using YourBrand.Accounting.Client;

using YourBrand.Documents.Client;

using YourBrand.Invoicing.Client;
using YourBrand.Invoicing.Contracts;

using MassTransit;
using YourBrand.RotRutService.Domain;

namespace YourBrand.RotRutService.Consumers;

public class InvoicePaidConsumer : IConsumer<InvoicePaid>
{
    private readonly IRotRutContext _context;
    private readonly IJournalEntriesClient _verificationsClient;
    private readonly IInvoicesClient _invoicesClient;
    private readonly RotRutCaseFactory _rotRutCaseFactory;

    public InvoicePaidConsumer(IRotRutContext context, IJournalEntriesClient verificationsClient,
        IInvoicesClient invoicesClient, RotRutCaseFactory rotRutCaseFactory)
    {
        _context = context;
        _verificationsClient = verificationsClient;
        _invoicesClient = invoicesClient;
        _rotRutCaseFactory = rotRutCaseFactory;
    }

    public async Task Consume(ConsumeContext<InvoicePaid> context)
    {
        var invoice = await _invoicesClient.GetInvoiceAsync(context.Message.Id, context.CancellationToken);

        await CreateRotRutCase(invoice, context.CancellationToken);
    }

    private async Task CreateRotRutCase(YourBrand.Invoicing.Client.Invoice invoice, CancellationToken cancellationToken)
    {
        var domesticServices = invoice?.DomesticService;

        if (domesticServices is not null)
        {
            var rotRutCase = _rotRutCaseFactory.CreateRotRutCase(invoice);

            _context.RotRutCases.Add(rotRutCase);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }

    /*
    private async Task DeleteRotRutCase(Domain.Entities.Invoice? invoice, CancellationToken cancellationToken)
    {
        var domesticServices = invoice?.DomesticService;
        if (domesticServices is not null)
        {
            var rotRutCase = await _context.RotRutCases.FirstAsync(x => x.InvoiceId == invoice.Id, cancellationToken);

            _context.RotRutCases.Remove(rotRutCase);

            await _context.SaveChangesAsync(cancellationToken);
        }
    }
    */
}