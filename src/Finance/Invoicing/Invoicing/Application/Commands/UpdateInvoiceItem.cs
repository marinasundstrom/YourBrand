using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Application.Commands;

public record UpdateInvoiceItem(string OrganizationId, string InvoiceId, string InvoiceItemId, ProductType ProductType, string Description, string? ProductId, decimal UnitPrice, string Unit, decimal? Discount, double VatRate, double Quantity, bool IsTaxDeductibleService) : IRequest<InvoiceItemDto>
{
    public class Handler(IInvoicingContext context, TimeProvider timeProvider) : IRequestHandler<UpdateInvoiceItem, InvoiceItemDto>
    {
        private readonly IInvoicingContext _context = context;

        public async Task<InvoiceItemDto> Handle(UpdateInvoiceItem request, CancellationToken cancellationToken)
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

            var item = invoice.Items.FirstOrDefault(x => x.Id == request.InvoiceItemId);

            if (item is null)
            {
                throw new Exception("Not found");
            }

            item.ProductId = request.ProductId;

            item.UpdateProductType(request.ProductType);
            item.UpdateDescription(request.Description);
            item.UpdateUnitPrice(request.UnitPrice);
            item.UpdateUnit(request.Unit);
            item.UpdateVatRate(request.UnitPrice, request.VatRate, timeProvider);
            item.UpdateQuantity(request.Quantity, timeProvider);
            item.Discount = request.Discount;

            item.IsTaxDeductibleService = request.IsTaxDeductibleService;

            invoice.Update(timeProvider);

            await _context.SaveChangesAsync(cancellationToken);

            return item.ToDto();
        }
    }
}