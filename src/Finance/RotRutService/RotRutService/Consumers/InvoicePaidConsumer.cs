using MassTransit;

using YourBrand.Accounting.Client;
using YourBrand.Invoicing.Client;
using YourBrand.Invoicing.Contracts;
using YourBrand.RotRutService.Domain;

namespace YourBrand.RotRutService.Consumers;

public class InvoicePaidConsumer(IRotRutContext context, IJournalEntriesClient journalEntriesClient,
    IInvoicesClient invoicesClient, RotRutCaseFactory rotRutCaseFactory) : IConsumer<InvoicePaid>
{
    public async Task Consume(ConsumeContext<InvoicePaid> context)
    {
        string organizationId = "";

        var invoice = await invoicesClient.GetInvoiceAsync(organizationId, context.Message.Id, context.CancellationToken);

        await CreateRotRutCase(invoice, context.CancellationToken);
    }

    private async Task CreateRotRutCase(YourBrand.Invoicing.Client.Invoice invoice, CancellationToken cancellationToken)
    {
        var domesticServices = invoice?.DomesticService;

        if (domesticServices is not null)
        {
            var rotRutCase = rotRutCaseFactory.CreateRotRutCase(invoice);

            context.RotRutCases.Add(rotRutCase);

            await context.SaveChangesAsync(cancellationToken);
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