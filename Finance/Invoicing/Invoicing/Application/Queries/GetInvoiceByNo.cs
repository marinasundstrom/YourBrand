using YourBrand.Invoicing.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Invoicing.Application.Queries;

public record GetInvoiceByNo(string InvoiceNo) : IRequest<InvoiceDto?>
{
    public class Handler : IRequestHandler<GetInvoiceByNo, InvoiceDto?>
    {
        private readonly IInvoicingContext _context;

        public Handler(IInvoicingContext context)
        {
            _context = context;
        }

        public async Task<InvoiceDto?> Handle(GetInvoiceByNo request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.InvoiceNo == request.InvoiceNo, cancellationToken);

            return invoice is null
                ? null
                : invoice.ToDto();
        }
    }
}