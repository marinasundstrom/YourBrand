using System.ComponentModel.DataAnnotations;

using YourBrand.Invoicing.Domain.Enums;

namespace YourBrand.Invoicing.Domain.Entities;

public class InvoiceItem 
{
    private InvoiceItem()
    {

    }

    public InvoiceItem(Invoice invoice, ProductType productType, string description, decimal unitPrice, string unit, double vatRate, double quantity)
    {
        Invoice = invoice;
        ProductType = productType;
        Description = description;
        Unit = unit;
        UpdateVatRate(unitPrice, vatRate);
        UpdateQuantity(quantity);
    }

    public int Id { get; private set; }

    public Invoice Invoice { get; private set; }

    public int InvoiceId { get; private set; }

    public string? ProductId { get; private set; }

    public ProductType ProductType  { get; private set; }

    public void UpdateProductType(ProductType productType)
    {
        if (productType != ProductType)
        {
            ProductType = productType;
        }
    }

    public string Description { get; private set; } = null!;

    public void UpdateDescription(string description) 
    {
        Description = description;
    }

    public string Unit { get; private set; } = null!;

    public void UpdateUnit(string unit)
    {
        if (unit != Unit)
        {
            Unit = unit;
        }
    }

    public double Quantity { get; private set; }

    public void UpdateQuantity(double quantity) 
    {
        Quantity = quantity;

        LineTotal = UnitPrice * (decimal)Quantity;

        Invoice?.UpdateTotals();
    }
    
    public decimal UnitPrice { get; private set; }

    public void UpdateUnitPrice(decimal unitPrice)
    {
        if (unitPrice != UnitPrice)
        {
            UnitPrice = unitPrice;
        }
    }

    public double VatRate { get; private set; }

    public void UpdateVatRate(decimal unitPrice, double vatRate) 
    {
        UnitPrice = unitPrice;
        VatRate = vatRate;

        LineTotal = UnitPrice * (decimal)Quantity;

        Invoice?.UpdateTotals();
    }

    public decimal LineTotal { get; private set; }

    public bool IsTaxDeductibleService { get; set; }

    public InvoiceItemDomesticService? DomesticService { get; set; }
}

public record InvoiceItemDomesticService(DomesticServiceKind Kind, HomeRepairAndMaintenanceServiceType? HomeRepairAndMaintenanceServiceType, HouseholdServiceType? HouseholdServiceType);

/* , double Hours, decimal LaborCost, decimal MaterialCost, decimal OtherCosts, decimal RequestedAmount */

[Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
public enum PropertyType 
{
    [Display(Name = "Villa")]
    HousingUnit = 1,

    [Display(Name = "Bostadsr??tt")]
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

    [Display(Name = "Dr??nering")]
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

    [Display(Name = "Flyttj??nst")]
    MovingService,
    
    [Display(Name = "It-tj??nster")]
    ItServices,

    [Display(Name = "Kl??d och textilv??rd")]
    ClothingAndTextileCare,

    [Display(Name = "Personlig omsorg")]
    PersonalCare,

    [Display(Name = "Reparation av vitvaror")]
    RepairOfHoushouldAppliances,

    [Display(Name = "Sn??skottning")]
    SnowShoveling,

    [Display(Name = "St??dning")]
    Cleaning,

    [Display(Name = "Tr??dg??rdsarbete")]
    Gardening
}