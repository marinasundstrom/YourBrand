
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Invoicing.Domain;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Application.Commands;

public record AddItem(string OrganizationId, string InvoiceId, ProductType ProductType, string Description, string? ProductId, decimal UnitPrice, string Unit, List<CreateInvoiceItemOptionDto> Options, List<CreateDiscountDto> Discounts, double VatRate, double Quantity, bool? IsTaxDeductibleService, InvoiceItemDomesticServiceDto? DomesticService) : IRequest<InvoiceItemDto>
{
    public class Handler(IInvoicingContext context, TimeProvider timeProvider) : IRequestHandler<AddItem, InvoiceItemDto>
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

            var item = invoice.AddItem(request.ProductType, request.Description, request.ProductId, request.UnitPrice, request.Unit, request.VatRate, request.Quantity, timeProvider);

            item.IsTaxDeductibleService = request.IsTaxDeductibleService.GetValueOrDefault();

            if (request.DomesticService is not null)
            {
                item.DomesticService = new Domain.Entities.InvoiceItemDomesticService(
                    request.DomesticService.Kind,
                    request.DomesticService.HomeRepairAndMaintenanceServiceType,
                    request.DomesticService.HouseholdServiceType
                );
            }

            if (request.Options is not null)
            {
                foreach (var option in request.Options)
                {
                    item.AddOption(option.Name, option.Description, option.Value, option.ProductId, option.ItemId, option.Price, null, timeProvider);
                }
            }

            if (request.Discounts is not null)
            {
                foreach (var discount in request.Discounts)
                {
                    item.AddPromotionalDiscount(discount.Description, discount.Amount, discount.Rate, timeProvider);
                }
            }

            await _context.SaveChangesAsync(cancellationToken);

            return item.ToDto();
        }
    }
}


public class CreateDiscountDto
{
    public string Description { get; set; } = null!;

    public double? Rate { get; set; }

    public decimal? Amount { get; set; }

    public decimal? Total { get; set; }

}

public class CreateInvoiceItemOptionDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public string? Value { get; set; } = null!;

    public string? ProductId { get; set; }

    public string? ItemId { get; set; }

    public decimal? Price { get; set; }
}