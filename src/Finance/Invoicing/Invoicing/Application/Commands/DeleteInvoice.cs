using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;

namespace YourBrand.Invoicing.Application.Commands;

public record DeleteInvoice(string InvoiceId) : IRequest
{
    public class Handler(IInvoicingContext context) : IRequestHandler<DeleteInvoice>
    {
        public async Task Handle(DeleteInvoice request, CancellationToken cancellationToken)
        {
            var invoice = await context.Invoices
                .Include(i => i.Items)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            if (invoice is null)
            {
                throw new Exception();
            }

            if (invoice.Status != Domain.Enums.InvoiceStatus.Draft)
            {
                throw new Exception();
            }

            invoice.VatAmounts.Clear();

            context.Invoices.Remove(invoice);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}