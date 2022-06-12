using System.ComponentModel.DataAnnotations;

using YourBrand.Invoices.Domain.Enums;

namespace YourBrand.Invoices.Domain.Entities;

public class InvoiceItem 
{
    public InvoiceItem(ProductType productType, string description, decimal unitPrice, string unit, double vatRate, double quantity)
    {
        ProductType = productType;
        Description = description;
        Unit = unit;
        UpdateUnitPrice(unitPrice, vatRate);
        UpdateQuantity(quantity);
    }

    public int Id { get; private set; }

    public Invoice Invoice { get; set; } = null!;

    public int InvoiceId { get; private set; }

    public string? ProductId { get; private set; }

    public ProductType ProductType  { get; private set; }

    public string Description { get; private set; } = null!;

    public void UpdateDescription(string description) 
    {
        Description = description;
    }

    public string Unit { get; set; }

    public double Quantity { get; private set; }

    public void UpdateQuantity(double quantity) 
    {
        Quantity = quantity;

        LineTotal = UnitPrice * (decimal)Quantity;

        Invoice?.UpdateTotals();
    }
    
    public decimal UnitPrice { get; private set; }

    public double VatRate { get; private set; }

    public void UpdateUnitPrice(decimal unitPrice, double vatRate) 
    {
        UnitPrice = unitPrice;
        VatRate = vatRate;

        LineTotal = UnitPrice * (decimal)Quantity;

        Invoice?.UpdateTotals();
    }

    public decimal LineTotal { get; set; }

    public decimal Vat()
    {
         return LineTotal - SubTotal();
    }

    public decimal SubTotal()
    {
        return LineTotal / (1m + (decimal)VatRate);
    }
    
    public bool IsTaxDeductablService { get; set; }

    public InvoiceItemDomesticService? DomesticService { get; set; }
}

public record InvoiceItemDomesticService(DomesticServiceKind Kind, HomeRepairAndMaintenanceServiceType? HomeRepairAndMaintenanceServiceType, HouseholdServiceType? HouseholdServiceType);

/* , double Hours, decimal LaborCost, decimal MaterialCost, decimal OtherCosts, decimal RequestedAmount */

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum PropertyType 
{
    [Display(Name = "Villa")]
    HousingUnit = 1,

    [Display(Name = "Bostadsrätt")]
    CooperativeFlat = 2
}

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum DomesticServiceKind 
{
    [Display(Name = "ROT")]
    HomeRepairAndMaintenanceServiceType = 1,

    [Display(Name = "RUT")]
    HouseholdService = 2
}

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum HomeRepairAndMaintenanceServiceType
{
    [Display(Name = "Bygg")]    
    Carpentry,

    [Display(Name = "El")]
    ElectricityWork,

    [Display(Name = "Glas och metalarbete")]
    GlassMetalWork,

    [Display(Name = "Tapetsering")]
    Wallpapering,

    [Display(Name = "Dränering")]
    DrainageWork,

    [Display(Name = "Murning")]
    Masonry,

    [Display(Name = "Vvs")]
    WaterAndHeating
}

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum HouseholdServiceType
{
    [Display(Name = "Barnpassning")]
    Childcare,

    [Display(Name = "Flyttjänst")]
    MovingService,
    
    [Display(Name = "It-tjänster")]
    ItServices,

    [Display(Name = "Kläd och textilvård")]
    ClothingAndTextileCare,

    [Display(Name = "Personlig omsorg")]
    PersonalCare,

    [Display(Name = "Reparation av vitvaror")]
    RepairOfHoushouldAppliances,

    [Display(Name = "Snöskottning")]
    SnowShoveling,

    [Display(Name = "Städning")]
    Cleaning,

    [Display(Name = "Trädgårdsarbete")]
    Gardening
}