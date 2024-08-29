using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Application.Commands;

public record ActivateRotAndRut(string OrganizationId, string InvoiceId, InvoiceDomesticServiceDto? DomesticService) : IRequest<InvoiceDto>
{
    public class Handler(IInvoicingContext context) : IRequestHandler<ActivateRotAndRut, InvoiceDto>
    {
        public async Task<InvoiceDto> Handle(ActivateRotAndRut request, CancellationToken cancellationToken)
        {
            var invoice = await context.Invoices
                .Include(i => i.Items)
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            if (invoice is null)
            {
                throw new Exception("Not found");
            }

            if (invoice.StatusId != (int)Domain.Enums.InvoiceStatus.Draft)
            {
                throw new Exception();
            }

            if (request.DomesticService is not null)
            {
                invoice.DomesticService = new Domain.Entities.InvoiceDomesticService(
                  request.DomesticService.Kind,
                  request.DomesticService.Buyer,
                  request.DomesticService.Description,
                  request.DomesticService.RequestedAmount
                );

                invoice.DomesticService.PropertyDetails = request.DomesticService.PropertyDetails is not null
                        ? new Domain.Entities.PropertyDetails(
                            request.DomesticService.PropertyDetails.Type,
                            request.DomesticService.PropertyDetails.PropertyDesignation,
                            request.DomesticService.PropertyDetails.ApartmentNo,
                            request.DomesticService.PropertyDetails.OrganizationNo
                        ) : null;
            }

            invoice.Update();

            await context.SaveChangesAsync(cancellationToken);

            return invoice.ToDto();
        }
    }
}