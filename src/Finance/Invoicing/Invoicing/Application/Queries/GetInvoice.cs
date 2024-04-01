using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;

namespace YourBrand.Invoicing.Application.Queries;

public record GetInvoice(string InvoiceId) : IRequest<InvoiceDto?>
{
    public class Handler : IRequestHandler<GetInvoice, InvoiceDto?>
    {
        private readonly IInvoicingContext _context;

        public Handler(IInvoicingContext context)
        {
            _context = context;
        }

        public async Task<InvoiceDto?> Handle(GetInvoice request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            return invoice is null
                ? null
                : invoice.ToDto();
        }
    }
}