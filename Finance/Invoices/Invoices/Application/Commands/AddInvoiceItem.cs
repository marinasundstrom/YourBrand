
using YourBrand.Invoices.Application;
using YourBrand.Invoices.Domain;
using YourBrand.Invoices.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Invoices.Application.Commands;

public record AddItemItem(int InvoiceId, ProductType ProductType, string Description, decimal UnitPrice, string Unit, double VatRate, double Quantity, InvoiceItemDomesticServiceDto? DomesticService) : IRequest<InvoiceItemDto>
{
    public class Handler : IRequestHandler<AddItemItem, InvoiceItemDto>
    {
        private readonly IInvoicesContext _context;

        public Handler(IInvoicesContext context)
        {
            _context = context;
        }

        public async Task<InvoiceItemDto> Handle(AddItemItem request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
                .Include(i => i.Items)
                .FirstOrDefaultAsync(x => x.Id == request.InvoiceId, cancellationToken);

            if(invoice is null) 
            {
                throw new Exception("Not found");
            }

            if(invoice.Status != InvoiceStatus.Draft) 
            {
                throw new Exception();
            }

            var item = invoice.AddItem(request.ProductType, request.Description, request.UnitPrice, request.Unit, request.VatRate, request.Quantity);

            if(request.DomesticService is not null)
            {
                item.DomesticService = new Domain.Entities.InvoiceItemDomesticService(
                    (Domain.Entities.DomesticServiceKind)request.DomesticService.Kind,
                    (Domain.Entities.HomeRepairAndMaintenanceServiceType?)request.DomesticService.HomeRepairAndMaintenanceServiceType,
                    (Domain.Entities.HouseholdServiceType?)request.DomesticService.HouseholdServiceType
                );
            }

            /*
                item.DomesticService.PropertyDetails = request.DomesticService.PropertyDetails is not null 
                        ? new Domain.Entities.PropertyDetails(
                            (Domain.Entities.PropertyType)request.DomesticService.PropertyDetails.Type,
                            request.DomesticService.PropertyDetails.PropertyDesignation,
                            request.DomesticService.PropertyDetails.ApartmentNo,
                            request.DomesticService.PropertyDetails.OrganizationNo
                        ) : null;
                        */

            await _context.SaveChangesAsync(cancellationToken);

            return item.ToDto();
        }
    }
}