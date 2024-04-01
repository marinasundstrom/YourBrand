
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;

namespace YourBrand.Invoicing.Application.Commands;

public record DeleteInvoiceItem(string InvoiceId, string InvoiceItemId) : IRequest
{
    public class Handler : IRequestHandler<DeleteInvoiceItem>
    {
        private readonly IInvoicingContext _context;

        public Handler(IInvoicingContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteInvoiceItem request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            if (invoice is null)
            {
                throw new Exception("Not found");
            }

            var item = invoice.Items.First(i => i.Id == request.InvoiceItemId);

            invoice.DeleteItem(item);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}