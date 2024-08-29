
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;

namespace YourBrand.Invoicing.Application.Commands;

public record DeleteInvoiceItem(string OrganizationId, string InvoiceId, string InvoiceItemId) : IRequest
{
    public class Handler(IInvoicingContext context) : IRequestHandler<DeleteInvoiceItem>
    {
        public async Task Handle(DeleteInvoiceItem request, CancellationToken cancellationToken)
        {
            var invoice = await context.Invoices
                .Include(i => i.Items)
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            if (invoice is null)
            {
                throw new Exception("Not found");
            }

            var item = invoice.Items.First(i => i.Id == request.InvoiceItemId);

            invoice.DeleteItem(item);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}