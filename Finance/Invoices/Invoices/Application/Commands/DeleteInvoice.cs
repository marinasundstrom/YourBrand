using YourBrand.Invoices.Application.Queries;
using YourBrand.Invoices.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Invoices.Application.Commands;

public record DeleteInvoice(int InvoiceId) : IRequest
{
    public class Handler : IRequestHandler<DeleteInvoice>
    {
        private readonly IInvoicesContext _context;

        public Handler(IInvoicesContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteInvoice request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            if(invoice is null)
            {
                throw new Exception();
            }

            if(invoice.Status != Domain.Enums.InvoiceStatus.Draft)
            {
                throw new Exception();
            }

            _context.Invoices.Remove(invoice);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}