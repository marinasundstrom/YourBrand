using System.Collections.Generic;

using Core;

using YourBrand.Auditability;
using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public class ProductPrice : IAuditableEntity<string>, ISoftDeletableWithAudit, IHasTenant, IHasOrganization
{
    readonly HashSet<ProductPriceTier> _priceTiers = new();

    decimal _price;
    decimal? _regularPrice;

    protected ProductPrice() { }

    public ProductPrice(
        int productId, int? regionId, string? countryId, string currencyCode,
        decimal price, double? vatRate, int? vatRateId,  decimal? regularPrice = null)
    {
        Id = Guid.NewGuid().ToString();
        
        ProductId = productId;
        RegionId = regionId;
        CountryCode = countryId;
        CurrencyCode = currencyCode;
        _price = price;
        VatRate = vatRate;
        VatRateId = vatRateId;
        _regularPrice = regularPrice;
    }

    public string Id { get; set; }
    
    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public int ProductId { get; }

    public Product Product { get; }

    public int? RegionId { get; set; }

    public Region? Region { get; set; }

    public string? CountryCode { get; set; }
    
    public Country? Country { get; set; }

    public string CurrencyCode { get; set; }

    public Currency Currency { get; set; }

    public decimal Price
    {
        get => _price;
        internal set
        {
            if (RegularPrice is not null && value >= RegularPrice)
                throw new ArgumentException("Price cannot exceed or equal the Regular Price.");

            _price = value;
        }
    }

    public double? VatRate { get; set; }

    public int? VatRateId { get; set; }

    public decimal? Discount { get; private set; }

    public double? DiscountRate { get; private set; }

    public decimal? RegularPrice
    {
        get => _regularPrice;
        internal set
        {
            if (value is not null && value < Price)
                throw new ArgumentException("Regular Price cannot be less than Price.");

            _regularPrice = value;
        }
    }

    public void SetPrice(decimal price)
    {
        Price = price;

        if (RegularPrice is not null)
        {
            DiscountRate = PriceCalculations.CalculateDiscountRate(Price, RegularPrice.GetValueOrDefault());
            Discount = RegularPrice - Price;
        }
    }

    public void SetDiscountPrice(decimal discountPrice)
    {
        if (discountPrice >= Price)
            throw new ArgumentException("Discount price must be less than the base price.");

        RegularPrice = Price;
        Price = discountPrice;
        DiscountRate = PriceCalculations.CalculateDiscountRate(Price, RegularPrice.GetValueOrDefault());
        Discount = RegularPrice - Price;
    }

    public void ApplyPercentageDiscount(double percentage)
    {
        if (percentage < 0 || percentage > 100)
            throw new ArgumentOutOfRangeException(nameof(percentage), "Discount percentage must be between 0 and 100.");

        if (!RegularPrice.HasValue)
            throw new InvalidOperationException("Regular Price must be set before applying a discount percentage.");

        DiscountRate = percentage;
        Price = PriceCalculations.CalculateDiscountedPrice(RegularPrice.GetValueOrDefault(), percentage);
        Discount = RegularPrice - Price;
    }

    public void RestoreRegularPrice()
    {
        if (RegularPrice.HasValue)
        {
            Price = RegularPrice.Value;
            RegularPrice = null;
            DiscountRate = null;
            Discount = null;
        }
    }

    public DateTimeOffset? ValidFrom { get; set; }
    public DateTimeOffset? ValidTo { get; set; }

    public IReadOnlyCollection<ProductPriceTier> PriceTiers => _priceTiers;

    public void AddPriceTier(ProductPriceTier priceTier)
    {
        ArgumentNullException.ThrowIfNull(priceTier);

        if (priceTier.ProductPriceId != Id)
        {
            throw new InvalidOperationException("The price tier must belong to this price instance.");
        }

        _priceTiers.Add(priceTier);
    }

    public ProductPriceTier? GetPriceTier(string tierId)
    {
        ArgumentNullException.ThrowIfNull(tierId);

        return _priceTiers.FirstOrDefault(x => x.Id == tierId);
    }

    public void UpdatePriceTier(ProductPriceTier priceTier, int fromQuantity, int? toQuantity, ProductPriceTierType tierType, decimal value)
    {
        ArgumentNullException.ThrowIfNull(priceTier);

        if (!_priceTiers.Contains(priceTier))
        {
            throw new InvalidOperationException("The price tier must belong to this price instance.");
        }

        priceTier.Update(fromQuantity, toQuantity, tierType, value);
    }

    public void RemovePriceTier(ProductPriceTier priceTier)
    {
        ArgumentNullException.ThrowIfNull(priceTier);

        _priceTiers.Remove(priceTier);
    }

    public UserId? CreatedById { get; set; }
    public DateTimeOffset Created { get; set; }
    public UserId? LastModifiedById { get; set; }
    public DateTimeOffset? LastModified { get; set; }

    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
}