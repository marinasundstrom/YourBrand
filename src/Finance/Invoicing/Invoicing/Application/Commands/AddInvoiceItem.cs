
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Application.Commands;

public record AddItem(string OrganizationId, string InvoiceId, ProductType ProductType, string Description, string? ProductId, decimal UnitPrice, string Unit, decimal? Discount, double VatRate, double Quantity, bool? IsTaxDeductibleService, InvoiceItemDomesticServiceDto? DomesticService) : IRequest<InvoiceItemDto>
{
    public class Handler(IInvoicingContext context) : IRequestHandler<AddItem, InvoiceItemDto>
    {
        private readonly IInvoicingContext _context = context;

        public async Task<InvoiceItemDto> Handle(AddItem request, CancellationToken cancellationToken)
        {
            var invoice = await _context.Invoices
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

            var item = invoice.AddItem(request.ProductType, request.Description, request.ProductId, request.UnitPrice, request.Unit, request.Discount, request.VatRate, request.Quantity);

            item.IsTaxDeductibleService = request.IsTaxDeductibleService.GetValueOrDefault();

            if (request.DomesticService is not null)
            {
                item.DomesticService = new Domain.Entities.InvoiceItemDomesticService(
                    request.DomesticService.Kind,
                    request.DomesticService.HomeRepairAndMaintenanceServiceType,
                    request.DomesticService.HouseholdServiceType
                );
            }

            await _context.SaveChangesAsync(cancellationToken);

            return item.ToDto();
        }
    }
}