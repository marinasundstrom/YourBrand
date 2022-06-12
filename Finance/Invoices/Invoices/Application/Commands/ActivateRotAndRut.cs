using YourBrand.Invoices.Domain;
using YourBrand.Invoices.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Invoices.Application.Commands;

public record ActivateRotAndRut(int InvoiceId, InvoiceDomesticServiceDto? DomesticService) : IRequest<InvoiceDto>
{
    public class Handler : IRequestHandler<ActivateRotAndRut, InvoiceDto>
    {
        private readonly IInvoicesContext _context;

        public Handler(IInvoicesContext context)
        {
            _context = context;
        }

        public async Task<InvoiceDto> Handle(ActivateRotAndRut request, CancellationToken cancellationToken)
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

            if (request.DomesticService is not null)
            {

                invoice.DomesticService = new Domain.Entities.InvoiceDomesticService(
                  request.DomesticService.Kind,
                  request.DomesticService.Buyer,
                  request.DomesticService.Description
              );

                invoice.DomesticService.PropertyDetails = request.DomesticService.PropertyDetails is not null
                        ? new Domain.Entities.PropertyDetails(
                            request.DomesticService.PropertyDetails.Type,
                            request.DomesticService.PropertyDetails.PropertyDesignation,
                            request.DomesticService.PropertyDetails.ApartmentNo,
                            request.DomesticService.PropertyDetails.OrganizationNo
                        ) : null;
            }

            invoice.UpdateTotals();

            await _context.SaveChangesAsync(cancellationToken);

            return invoice.ToDto();
        }
    }
}