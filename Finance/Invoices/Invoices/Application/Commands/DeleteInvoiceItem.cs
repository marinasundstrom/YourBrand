
using YourBrand.Invoices.Application;
using YourBrand.Invoices.Domain;
using YourBrand.Invoices.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Invoices.Application.Commands;

public record DeleteInvoiceItem(int InvoiceId, int InvoiceItemId) : IRequest
{
    public class Handler : IRequestHandler<DeleteInvoiceItem>
    {
        private readonly IInvoicesContext _context;

        public Handler(IInvoicesContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteInvoiceItem request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            if(invoice is null) 
            {
                throw new Exception("Not found");
            }

            var item = invoice.Items.First(i => i.Id == request.InvoiceItemId);

            invoice.DeleteItem(item);
            
            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
