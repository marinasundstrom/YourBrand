using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;

namespace YourBrand.Sales.Features.InvoiceManagement.Invoices.Statuses.Commands;

public record DeleteInvoiceStatusCommand(string OrganizationId, int Id) : IRequest
{
    public class DeleteInvoiceStatusCommandHandler(IInvoicingContext context) : IRequestHandler<DeleteInvoiceStatusCommand>
    {
        private readonly IInvoicingContext context = context;

        public async Task Handle(DeleteInvoiceStatusCommand request, CancellationToken cancellationToken)
        {
            var invoiceStatus = await context.InvoiceStatuses
                .Where(x => x.OrganizationId == request.OrganizationId)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (invoiceStatus is null) throw new Exception();

            context.InvoiceStatuses.Remove(invoiceStatus);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}