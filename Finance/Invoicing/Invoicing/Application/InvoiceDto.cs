using System;

using YourBrand.Invoicing.Domain.Entities;
using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Application;

public record InvoiceDto(
    string Id, 
    string? InvoiceNo,
    DateTime? IssueDate, 
    Domain.Enums.InvoiceType Type, 
    Domain.Enums.InvoiceStatus Status, 
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
    decimal Total, 
    decimal? Paid, 
    InvoiceDomesticServiceDto? DomesticService);

public sealed record InvoiceVatAmountDto(string Name, double VatRate, decimal SubTotal, decimal? Vat, decimal Total);


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
    decimal Price, 
    string Unit, 
    double VatRate, 
    double Quantity,
    decimal Vat,
    decimal Total,
    bool IsTaxDeductibleService,
    InvoiceItemDomesticServiceDto? DomesticService);

public record InvoiceItemDomesticServiceDto(
    DomesticServiceKind Kind,
    HomeRepairAndMaintenanceServiceType? HomeRepairAndMaintenanceServiceType,
    HouseholdServiceType? HouseholdServiceType);

public record PropertyDetailsDto(Domain.Entities.PropertyType Type, string? PropertyDesignation, string? ApartmentNo, string? OrganizationNo);