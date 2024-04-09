using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;

namespace YourBrand.Invoicing.Application.Queries;

public record GetInvoiceByNo(string InvoiceNo) : IRequest<InvoiceDto?>
{
    public class Handler(IInvoicingContext context) : IRequestHandler<GetInvoiceByNo, InvoiceDto?>
    {
        public async Task<InvoiceDto?> Handle(GetInvoiceByNo request, CancellationToken cancellationToken)
        {
            var invoice = await context.Invoices
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