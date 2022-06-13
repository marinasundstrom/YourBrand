using YourBrand.RotRutService.Domain;
using YourBrand.RotRutService.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.RotRutService.Application.Commands;

/*
public record ActivateRotAndRut(int InvoiceId, InvoiceDomesticServiceDto? DomesticService) : IRequest<InvoiceDto>
{
    public class Handler : IRequestHandler<ActivateRotAndRut, InvoiceDto>
    {
        private readonly IRotRutContext _context;

        public Handler(IRotRutContext context)
        {
            _context = context;
        }

        public async Task<InvoiceDto> Handle(ActivateRotAndRut request, CancellationToken cancellationToken)
        {
            var invoice = await _context.RotRutService
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
*/