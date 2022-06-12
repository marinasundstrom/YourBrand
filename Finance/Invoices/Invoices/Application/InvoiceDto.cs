using System;

using YourBrand.Invoices.Domain.Entities;
using YourBrand.Invoices.Domain.Enums;

namespace YourBrand.Invoices.Application;

public record InvoiceDto(int Id, DateTime? Date, Domain.Enums.InvoiceType Type, Domain.Enums.InvoiceStatus Status, DateTime? DueDate, string Currency, string? Reference, string? Note, IEnumerable<InvoiceItemDto> Items, decimal SubTotal, decimal Vat, decimal Total, decimal? Paid, InvoiceDomesticServiceDto? DomesticService);

public record InvoiceDomesticServiceDto(
    Domain.Entities.DomesticServiceKind Type, 
    string Description,
    PropertyDetailsDto? PropertyDetails);

public record InvoiceItemDto(
    int Id, 
    ProductType ProductType, 
    string Description, 
    decimal UnitPrice, 
    string Unit, 
    double VatRate, 
    double Quantity, 
    decimal LineTotal, 
    bool IsTaxDeductablService, 
    InvoiceItemDomesticServiceDto? DomesticService);

public record InvoiceItemDomesticServiceDto(
    DomesticServiceKind Kind,
    HomeRepairAndMaintenanceServiceType? HomeRepairAndMaintenanceServiceType,
    HouseholdServiceType? HouseholdServiceType);

public record PropertyDetailsDto(Domain.Entities.PropertyType Type, string? PropertyDesignation, string? ApartmentNo, string? OrganizationNo);