using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Application.Commands;

public record CreateInvoiceItemDto(ProductType ProductType, string Description, string? ProductId, decimal UnitPrice, string Unit, List<CreateInvoiceItemOptionDto> Options, List<CreateDiscountDto> Discounts, double VatRate, double Quantity,
   bool? IsTaxDeductibleService, InvoiceItemDomesticServiceDto? DomesticService);
