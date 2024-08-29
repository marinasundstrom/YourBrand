using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;

namespace YourBrand.Invoicing.Application.Queries;

public record GetInvoice(string OrganizationId, string InvoiceId) : IRequest<InvoiceDto?>
{
    public class Handler(IInvoicingContext context) : IRequestHandler<GetInvoice, InvoiceDto?>
    {
        public async Task<InvoiceDto?> Handle(GetInvoice request, CancellationToken cancellationToken)
        {
            var invoice = await context.Invoices
                .Include(i => i.Status)
                .Include(i => i.Items)
                .AsSplitQuery()
                .AsNoTracking()
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            return invoice is null
                ? null
                : invoice.ToDto();
        }
    }
}