using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Application.Commands;

public record UpdateInvoiceItemQuantity(string InvoiceId, string InvoiceItemId, double Quantity) : IRequest<InvoiceItemDto>
{
    public class Handler : IRequestHandler<UpdateInvoiceItemQuantity, InvoiceItemDto>
    {
        private readonly IInvoicingContext _context;

        public Handler(IInvoicingContext context)
        {
            _context = context;
        }

        public async Task<InvoiceItemDto> Handle(UpdateInvoiceItemQuantity request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            if (invoice is null)
            {
                throw new Exception("Not found");
            }

            if (invoice.Status != InvoiceStatus.Draft)
            {
                throw new Exception();
            }

            var item = invoice.Items.FirstOrDefault(x => x.Id == request.InvoiceItemId);

            if (item is null)
            {
                throw new Exception("Not found");
            }

            item.UpdateQuantity(request.Quantity);

            invoice.Update();

            await _context.SaveChangesAsync(cancellationToken);

            return item.ToDto();
        }
    }
}