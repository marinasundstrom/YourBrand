using System.ComponentModel.DataAnnotations;

using YourBrand.Domain;
using YourBrand.Invoicing.Domain.Common;
using YourBrand.Invoicing.Domain.Enums;
using YourBrand.Tenancy;

namespace YourBrand.Invoicing.Domain.Entities;

public class InvoiceItem : AuditableEntity<string>, IHasTenant
{
    private InvoiceItem()
    {

    }

    public InvoiceItem(Invoice invoice, ProductType productType, string description, string? productId, decimal unitPrice, string unit, decimal? discount, double vatRate, double quantity)
     : base(Guid.NewGuid().ToString())
    {
        Invoice = invoice;
        ProductType = productType;
        Description = description;
        ProductId = productId;
        Unit = unit;
        Discount = discount;
        UpdateVatRate(unitPrice, vatRate);
        UpdateQuantity(quantity);
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public Invoice Invoice { get; private set; }

    public string InvoiceId { get; private set; }

    public string Description { get; private set; } = null!;

    public void UpdateDescription(string description)
    {
        Description = description;
    }

    public string? ProductId { get; set; }

    public string? Sku { get; set; }

    public ProductType ProductType { get; private set; }

    public void UpdateProductType(ProductType productType)
    {
        if (productType != ProductType)
        {
            ProductType = productType;
        }
    }

    public string Unit { get; private set; } = null!;


    public string? UnitId { get; set; }

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

        Total = Price * (decimal)Quantity;

        Invoice?.Update();
    }

    public decimal Price { get; private set; }

    public void UpdateUnitPrice(decimal unitPrice)
    {
        if (unitPrice != Price)
        {
            Price = unitPrice;
        }
    }

    public decimal? RegularPrice { get; set; }

    public double? DiscountRate { get; set; }

    public decimal? Discount { get; set; }

    public double? VatRate { get; private set; }

    public decimal? Vat { get; private set; }

    public void UpdateVatRate(decimal unitPrice, double vatRate)
    {
        Price = unitPrice;
        VatRate = vatRate;

        Total = Price * (decimal)Quantity;

        Invoice?.Update();
    }

    private readonly HashSet<InvoiceItemOption> _options = new HashSet<InvoiceItemOption>();
    public IReadOnlyCollection<InvoiceItemOption> Options => _options;


    public decimal Total { get; private set; }

    public string? Notes { get; private set; }

    public bool IsTaxDeductibleService { get; set; }

    public InvoiceItemDomesticService? DomesticService { get; set; }

    public void Update()
    {
        Total = Price * (decimal)Quantity;
        Vat = Math.Round(Total.GetVatFromTotal(VatRate.GetValueOrDefault()), 2, MidpointRounding.ToEven);
    }

    public InvoiceItemOption AddOption(string description, string? productId, string? itemId, decimal? price, decimal? discount)
    {
        var option = new InvoiceItemOption(description, productId, itemId, price, discount);
        _options.Add(option);
        Update();
        return option;
    }

    public void RemoveOption(InvoiceItemOption option) => _options.Remove(option);
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