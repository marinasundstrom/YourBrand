using YourBrand.Invoicing.Domain.Entities;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Application;

public record InvoiceDto(
    string Id,
    string OrganizationId,
    int? InvoiceNo,
    DateTimeOffset? IssueDate,
    Domain.Enums.InvoiceType Type,
    InvoiceStatusDto Status,
    DateTimeOffset? StatusDate,
    CustomerDto? Customer,
    DateTimeOffset? DueDate,
    string Currency,
    string? Reference,
    string? Note,
    BillingDetailsDto? BillingDetails,
    ShippingDetailsDto? ShippingDetails,
    IEnumerable<InvoiceItemDto> Items,
    decimal SubTotal,
    IEnumerable<InvoiceVatAmountDto> VatAmounts,
    double? VatRate,
    decimal? Vat,
    decimal? Discount,
    decimal Total,
    decimal? Paid,
    InvoiceDomesticServiceDto? DomesticService,
    DateTimeOffset Created,
    string? CreatedBy,
    DateTimeOffset? LastModified,
    string? LastModifiedById);

public record InvoiceStatusDto(
    int Id,
    string OrganizationId,
    string? Name);

public record CustomerDto(
    string Id,
    long CustomerNo,
    string Name);

public sealed record InvoiceVatAmountDto(
    string Name,
    double VatRate,
    decimal SubTotal,
    decimal? Vat,
    decimal Total);

public record InvoiceDomesticServiceDto(
    Domain.Entities.DomesticServiceKind Kind,
    string Buyer,
    string Description,
    decimal RequestedAmount,
    PropertyDetailsDto? PropertyDetails);

public record InvoiceItemDto(
    string Id,
    ProductType ProductType,
    string Description,
    string? ProductId,
    string? SKU,
    decimal UnitPrice,
    string? Unit,
    IEnumerable<InvoiceItemOptionDto> Options,
    IEnumerable<DiscountDto> Discounts,
    decimal? RegularPrice,
    double? VatRate,
    double Quantity,
    decimal? Vat,
    decimal Total,
    string? Notes,
    bool IsTaxDeductibleService,
    InvoiceItemDomesticServiceDto? DomesticService,
    DateTimeOffset Created,
    string? CreatedBy,
    DateTimeOffset? LastModified,
    string? LastModifiedBy);

public sealed record DiscountDto(    
    string Description,
    double? Rate,
    decimal? Amount,
    decimal? Total,
    DateTimeOffset? EffectiveDate,
    DateTimeOffset? ExpiryDate);

public sealed record InvoiceItemOptionDto(
    string Id,
    string Name,
    string? Description,
    string? Value,
    string? ProductId,
    string? ItemId,
    decimal? Price,
    decimal? Discount,
    DateTimeOffset Created,
    string? CreatedBy,
    DateTimeOffset? LastModified,
    string? LastModifiedBy);

public record InvoiceItemDomesticServiceDto(
    DomesticServiceKind Kind,
    HomeRepairAndMaintenanceServiceType? HomeRepairAndMaintenanceServiceType,
    HouseholdServiceType? HouseholdServiceType);

public record PropertyDetailsDto(Domain.Entities.PropertyType Type, string? PropertyDesignation, string? ApartmentNo, string? OrganizationNo);