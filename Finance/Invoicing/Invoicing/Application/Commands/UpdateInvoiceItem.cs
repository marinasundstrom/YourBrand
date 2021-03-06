using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Invoicing.Application.Commands;

public record UpdateInvoiceItem(int InvoiceId, int InvoiceItemId, ProductType ProductType, string Description, decimal UnitPrice, string Unit, double VatRate, double Quantity, bool IsTaxDeductibleService) : IRequest<InvoiceItemDto>
{
    public class Handler : IRequestHandler<UpdateInvoiceItem, InvoiceItemDto>
    {
        private readonly IInvoicingContext _context;

        public Handler(IInvoicingContext context)
        {
            _context = context;
        }

        public async Task<InvoiceItemDto> Handle(UpdateInvoiceItem request, CancellationToken cancellationToken)
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

            var item = invoice.Items.FirstOrDefault(x => x.Id == request.InvoiceItemId);

            if (item is null)
            {
                throw new Exception("Not found");
            }

            item.UpdateProductType(request.ProductType);
            item.UpdateDescription(request.Description);
            item.UpdateUnitPrice(request.UnitPrice);
            item.UpdateUnit(request.Unit);
            item.UpdateVatRate(request.UnitPrice, request.VatRate);
            item.UpdateQuantity(request.Quantity);

            item.IsTaxDeductibleService = request.IsTaxDeductibleService;

            await _context.SaveChangesAsync(cancellationToken);

            return item.ToDto();
        }
    }
}
