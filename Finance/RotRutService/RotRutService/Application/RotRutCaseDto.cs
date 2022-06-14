using YourBrand.RotRutService.Domain.Entities;
using YourBrand.RotRutService.Domain.Enums;

namespace YourBrand.RotRutService.Application;

public record RotRutCaseDto(
    DomesticServiceKind Kind, RotRutCaseStatus Status,string Buyer, DateTime PaymentDate,
    decimal LaborCost, decimal PaidAmount, decimal RequestedAmount,
    int InvoiceId, decimal OtherCosts, double Hours, decimal MaterialCost,
    decimal? ReceivedAmount, RotDto? Rot, RutDto? Rut);

public record RotDto(HomeRepairAndMaintenanceServiceType ServiceType, string PropertyDesignation, string? ApartmentNo, string? OrganizationNo);

public record RutDto(HouseholdServiceType ServiceType);