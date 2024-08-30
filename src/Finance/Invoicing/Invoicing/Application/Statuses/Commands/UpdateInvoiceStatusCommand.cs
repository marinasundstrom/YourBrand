using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;

namespace YourBrand.Sales.Features.InvoiceManagement.Invoices.Statuses.Commands;

public record UpdateInvoiceStatusCommand(string OrganizationId, int Id, string Name, string Handle, string? Description) : IRequest
{
    public class UpdateInvoiceStatusCommandHandler(IInvoicingContext context) : IRequestHandler<UpdateInvoiceStatusCommand>
    {
        private readonly IInvoicingContext context = context;

        public async Task Handle(UpdateInvoiceStatusCommand request, CancellationToken cancellationToken)
        {
            var invoiceStatus = await context.InvoiceStatuses
                    .Where(x => x.OrganizationId == request.OrganizationId)
                    .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (invoiceStatus is null) throw new Exception();

            invoiceStatus.Name = request.Name;
            invoiceStatus.Handle = request.Handle;
            invoiceStatus.Description = request.Description;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}