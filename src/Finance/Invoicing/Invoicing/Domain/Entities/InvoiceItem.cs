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

    public InvoiceItem(Invoice invoice, ProductType productType, string description, string? productId, decimal unitPrice, string unit, double vatRate, double quantity)
     : base(Guid.NewGuid().ToString())
    {
        Invoice = invoice;
        ProductType = productType;
        Description = description;
        ProductId = productId;
        Unit = unit;
        Price = unitPrice;
        Quantity = quantity;
        VatRate = vatRate;
        //VatIncluded = ;
        //Discount = discount;
        //UpdateVatRate(unitPrice, vatRate, timeProvider);
        //UpdateQuantity(quantity, timeProvider);
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public Invoice Invoice { get; private set; }

    public string InvoiceId { get; private set; }

    public int? OrderNo { get; private set; }

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

    public void UpdateQuantity(double quantity, TimeProvider timeProvider)
    {
        Quantity = quantity;

        Total = Price * (decimal)Quantity;

        Invoice?.Update(timeProvider);
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

    public bool VatIncluded { get; private set; }

    private readonly HashSet<InvoiceItemOption> _options = new HashSet<InvoiceItemOption>();
    public IReadOnlyCollection<InvoiceItemOption> Options => _options;

    public InvoiceItemOption AddOption(string name, string? description, string? value, string? productId, string? itemId, decimal? price, decimal? discount, TimeProvider timeProvider)
    {
        var option = new InvoiceItemOption(name, description, value, productId, itemId, price, discount);
        _options.Add(option);
        Update(timeProvider);
        return option;
    }

    public void RemoveOption(InvoiceItemOption option, TimeProvider timeProvider)
    {
        _options.Remove(option);
        Update(timeProvider);
    }

    // Base Price Discount (difference between RegularPrice and Price)
    public decimal BasePriceDiscount => RegularPrice.HasValue && RegularPrice > Price ? (RegularPrice.Value - Price) * (decimal)Quantity : 0;

    // Direct Discount applied directly to the item
    public decimal DirectDiscount { get; private set; } = 0;

    // Promotional Discounts
    private readonly HashSet<Discount> _promotionalDiscounts = new HashSet<Discount>();
    public IReadOnlyCollection<Discount> PromotionalDiscounts => _promotionalDiscounts;

    public decimal PromotionalDiscount => _promotionalDiscounts.Sum(d => d.Amount.GetValueOrDefault() * (decimal)Quantity);

    // Total Discount (sum of all discounts)
    public decimal TotalDiscount => BasePriceDiscount + DirectDiscount + PromotionalDiscount;

    // Methods for managing discounts
    public bool ApplyDirectDiscount(decimal discountAmount, TimeProvider timeProvider)
    {
        DirectDiscount = discountAmount;
        Update(timeProvider); // Recalculate totals
        return true;
    }

    public bool AddPromotionalDiscount(string description, decimal? amount, double? rate, TimeProvider timeProvider)
    {
        var discount = new Discount { OrganizationId = OrganizationId, Description = description, Amount = amount, /* DiscountId = discountId */ };

        if (_promotionalDiscounts.Add(discount))
        {
            Update(timeProvider); // Recalculate totals
            return true;
        }
        return false;
    }

    public bool RemovePromotionalDiscount(Discount discount, TimeProvider timeProvider)
    {
        if (_promotionalDiscounts.Remove(discount))
        {
            Update(timeProvider); // Recalculate totals
            return true;
        }
        return false;
    }

    public double? DiscountRate { get; set; }

    public decimal? Discount { get; set; }

    public decimal SubTotal { get; private set; }

    public double? VatRate { get; private set; }

    public decimal? Vat { get; private set; }

    public void UpdateVatRate(decimal unitPrice, double vatRate, TimeProvider timeProvider)
    {
        Price = unitPrice;
        VatRate = vatRate;

        Total = Price * (decimal)Quantity;

        Invoice?.Update(timeProvider);
    }

    public decimal Total { get; private set; }

    public string? Notes { get; private set; }

    public bool IsTaxDeductibleService { get; set; }

    public InvoiceItemDomesticService? DomesticService { get; set; }

    public void Update(TimeProvider timeProvider)
    {
        // Calculate base total after all discounts (before VAT adjustment)
        var baseTotal = (Price * (decimal)Quantity) - Options.Sum(x => x.Price.GetValueOrDefault()) - TotalDiscount;

        // Calculate VAT and Total based on VAT inclusion status
        CalculateVatAndTotal(baseTotal);
    }

    private void CalculateVatAndTotal(decimal baseTotal)
    {
        if (VatRate.HasValue)
        {
            if (VatIncluded)
            {
                // Extract VAT from baseTotal as VAT is included in Price
                Vat = PriceCalculations.CalculateVat(baseTotal, VatRate.Value);
                SubTotal = baseTotal - Vat.GetValueOrDefault();
                Total = baseTotal;
            }
            else
            {
                Vat = baseTotal * (decimal)VatRate.Value;
                SubTotal = baseTotal;
                Total = baseTotal + Vat.GetValueOrDefault();
            }
        }
        else
        {
            Vat = 0;
            SubTotal = baseTotal;
            Total = baseTotal;
        }
    }

    public void AdjustForVatInclusionChange(bool newVatIncluded, TimeProvider timeProvider)
    {
        if (VatIncluded == newVatIncluded) return; // No change needed if the VAT inclusion status is the same

        VatIncluded = newVatIncluded;

        decimal adjustedPrice = Price;

        if (VatRate.HasValue)
        {
            if (newVatIncluded)
            {
                // VAT was not included, but now needs to be included
                adjustedPrice = Price * (1 + (decimal)VatRate.Value);
            }
            else
            {
                // VAT was included, but now needs to be excluded
                adjustedPrice = Price / (1 + (decimal)VatRate.Value);
            }
        }

        Price = adjustedPrice;
        Update(timeProvider); // Recalculate totals with the adjusted price
    }
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