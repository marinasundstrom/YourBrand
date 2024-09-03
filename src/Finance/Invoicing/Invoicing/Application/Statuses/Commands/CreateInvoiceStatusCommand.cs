using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Application;
using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Entities;

namespace YourBrand.Sales.Features.InvoiceManagement.Invoices.Statuses.Commands;

public record CreateInvoiceStatusCommand(string OrganizationId, string Name, string Handle, string? Description) : IRequest<InvoiceStatusDto>
{
    public class CreateInvoiceStatusCommandHandler(IInvoicingContext context) : IRequestHandler<CreateInvoiceStatusCommand, InvoiceStatusDto>
    {
        private readonly IInvoicingContext context = context;

        public async Task<InvoiceStatusDto> Handle(CreateInvoiceStatusCommand request, CancellationToken cancellationToken)
        {
            var invoiceStatus = await context.InvoiceStatuses
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (invoiceStatus is not null) throw new Exception();

            int invoiceStatusNo = 1;

            try
            {
                invoiceStatusNo = await context.InvoiceStatuses
                    .InOrganization(request.OrganizationId)
                    .Where(x => x.OrganizationId == request.OrganizationId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            invoiceStatus = new InvoiceStatus(invoiceStatusNo, request.Name, request.Handle, request.Description);
            invoiceStatus.OrganizationId = request.OrganizationId;

            context.InvoiceStatuses.Add(invoiceStatus);

            await context.SaveChangesAsync(cancellationToken);

            return invoiceStatus.ToDto();
        }
    }
}