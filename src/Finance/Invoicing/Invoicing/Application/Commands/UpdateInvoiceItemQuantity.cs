using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Application.Commands;

public record UpdateInvoiceItemQuantity(string InvoiceId, string InvoiceItemId, double Quantity) : IRequest<InvoiceItemDto>
{
    public class Handler(IInvoicingContext context) : IRequestHandler<UpdateInvoiceItemQuantity, InvoiceItemDto>
    {
        public async Task<InvoiceItemDto> Handle(UpdateInvoiceItemQuantity request, CancellationToken cancellationToken)
        {
            var invoice = await context.Invoices
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

            await context.SaveChangesAsync(cancellationToken);

            return item.ToDto();
        }
    }
}