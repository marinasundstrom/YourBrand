using System.ComponentModel.DataAnnotations;

namespace Accounting.Domain.Enums;

public enum AccountGroup
{
    [Display(Name = "Rented premises")]
    RentedPremises = 50,

    [Display(Name = "Property costs")]
    PropertyCosts = 51,

    [Display(Name = "Hired fixed assets")]
    HiredFixedAssets = 52,

    [Display(Name = "Energy costs")]
    EnergyCosts = 53,

    [Display(Name = "Consumable equipment and supplies")]
    ConsumableEquipmentAndSupplies = 54,

    [Display(Name = "Repairs and maintenance")]
    RepairsAndMaintenance = 55,

    [Display(Name = "Transport equipment costs")]
    TransportEquipmentCosts = 56,

    [Display(Name = "Freight and transportation")]
    FreightAndTransportation = 57,

    [Display(Name = "Travel expenses")]
    TravelExpenses = 58,

    [Display(Name = "Advertising and PR")]
    AdvertisingAndPR = 59,

    [Display(Name = "Other selling expenses")]
    OtherSellingExpenses = 60,

    [Display(Name = "Office supplies and printed matter")]
    OfficeSuppliesAndPrintedMatter = 61,

    [Display(Name = "Telecommunications and postal services")]
    TelecommunicationsAndPostalServices = 62,

    [Display(Name = "Corporate insurance and other risk-related costs")]
    CorporateInsuranceAndOtherRiskRelatedCosts = 63,

    [Display(Name = "Costs of administration")]
    CostsOfAdministration = 64,

    [Display(Name = "Other external services")]
    OtherExternalServices = 65,

    [Display(Name = "(Unspecified account group)")]
    UnspecifiedAccountGroup1 = 66,

    [Display(Name = "(Unspecified account group)")]
    UnspecifiedAccountGroup2 = 67,

    [Display(Name = "Temporary employee")]
    TemporaryEmployee = 68,

    [Display(Name = "Other external expenses")]
    OtherExternalExpenses = 69,
}