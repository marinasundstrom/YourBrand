using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Application.Commands;

public record SetInvoiceStatus(string OrganizationId, string InvoiceId, int Status) : IRequest
{
    public class Handler(IInvoicingContext context) : IRequestHandler<SetInvoiceStatus>
    {
        public async Task Handle(SetInvoiceStatus request, CancellationToken cancellationToken)
        {
            var invoice = await context.Invoices
                .InOrganization(request.OrganizationId)
                .FirstAsync(x => x.Id == request.InvoiceId, cancellationToken);

            invoice.UpdateStatus(request.Status);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}