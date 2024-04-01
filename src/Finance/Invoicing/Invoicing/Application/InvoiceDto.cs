using YourBrand.Invoicing.Domain.Entities;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Application;

public record InvoiceDto(
    string Id,
    string? InvoiceNo,
    DateTime? IssueDate,
    Domain.Enums.InvoiceType Type,
    Domain.Enums.InvoiceStatus Status,
    CustomerDto? Customer,
    DateTime? DueDate,
    string Currency,
    string? Reference,
    string? Note,
    BillingDetailsDto? BillingDetails,
    ShippingDetailsDto? ShippingDetails,
    IEnumerable<InvoiceItemDto> Items,
    decimal SubTotal,
    IEnumerable<InvoiceVatAmountDto> VatAmounts,
    decimal Vat,
    decimal? Discount,
    decimal Total,
    decimal? Paid,
    InvoiceDomesticServiceDto? DomesticService);

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
    decimal? Discount,
    decimal? RegularPrice,
    double? VatRate,
    double Quantity,
    decimal? Vat,
    decimal Total,
    string? Notes,
    bool IsTaxDeductibleService,
    InvoiceItemDomesticServiceDto? DomesticService);

public record InvoiceItemDomesticServiceDto(
    DomesticServiceKind Kind,
    HomeRepairAndMaintenanceServiceType? HomeRepairAndMaintenanceServiceType,
    HouseholdServiceType? HouseholdServiceType);

public record PropertyDetailsDto(Domain.Entities.PropertyType Type, string? PropertyDesignation, string? ApartmentNo, string? OrganizationNo);