
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;

namespace YourBrand.Invoicing.Application.Commands;

public record CreateInvoiceDraft(string OrganizationId) : IRequest<InvoiceDto>
{
    public class Handler(IInvoicingContext context, TimeProvider timeProvider) : IRequestHandler<CreateInvoiceDraft, InvoiceDto>
    {
        public async Task<InvoiceDto> Handle(CreateInvoiceDraft request, CancellationToken cancellationToken)
        {
            var invoice = new YourBrand.Invoicing.Domain.Entities.Invoice(Domain.Enums.InvoiceType.Invoice);

            invoice.SetCurrency("SEK");

            //invoice.Id = Guid.NewGuid().ToString();

            invoice.OrganizationId = request.OrganizationId;

            context.Invoices.Add(invoice);

            await context.SaveChangesAsync(cancellationToken);

            invoice = await context.Invoices
                .Include(i => i.Status)
                .Include(i => i.Items)
                .AsSplitQuery()
                .AsNoTracking()
                .InOrganization(request.OrganizationId)
                .FirstAsync(x => x.Id == invoice.Id, cancellationToken);

            return invoice.ToDto();
        }
    }
}