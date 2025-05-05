using Core;

using YourBrand.Catalog.Domain.Enums;
using YourBrand.Domain;
using YourBrand.Tenancy;

namespace YourBrand.Catalog.Domain.Entities;

public sealed class Product : Entity<int>, IHasTenant, IHasOrganization, IHasStore, IHasBrand
{
    readonly HashSet<ProductAttribute> _productAttributes = new HashSet<ProductAttribute>();

    readonly HashSet<AttributeGroup> _attributeGroups = new HashSet<AttributeGroup>();

    readonly HashSet<Product> _variants = new HashSet<Product>();

    readonly HashSet<Option> _options = new HashSet<Option>();

    readonly HashSet<ProductOption> _productOptions = new HashSet<ProductOption>();

    readonly HashSet<OptionGroup> _optionGroups = new HashSet<OptionGroup>();

    readonly HashSet<ProductVariantOption> _productVariantOptions = new HashSet<ProductVariantOption>();

    readonly HashSet<ProductImage> _images = new HashSet<ProductImage>();

    readonly HashSet<ProductSubscriptionPlan> _subscriptionPlans = new HashSet<ProductSubscriptionPlan>();

    private decimal _price;
    private decimal? _regularPrice;

    public Product() { }

    public Product(string name, string handle)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Handle = handle ?? throw new ArgumentNullException(nameof(handle));
    }

    public Product(OrganizationId organizationId, int id, string name) : base(id)
    {
        OrganizationId = organizationId;
        Name = name;
    }

    public Product(
        OrganizationId organizationId,
        Store store,
        string storeId,
        string name,
        string handle,
        decimal price,
        ProductType? type = ProductType.Good,
        string description = "",
        decimal? regularPrice = null,
        Brand? brand = null,
        ProductCategory? category = null)
    {
        OrganizationId = organizationId;
        Store = store;
        StoreId = storeId ?? throw new ArgumentNullException(nameof(storeId));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Handle = handle ?? throw new ArgumentNullException(nameof(handle));
        Price = price;
        Type = type;
        Description = description;
        RegularPrice = regularPrice;
        Brand = brand;
        Category = category;

        // Validate that price is not higher than regular price if regular price is set
        if (regularPrice.HasValue && price >= regularPrice)
            throw new ArgumentException("Price cannot be greater than or equal to Regular Price.");
    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; }

    public Store Store { get; set; }

    public string StoreId { get; set; }

    public Brand? Brand { get; set; }

    public int? BrandId { get; set; }

    public Producer? Producer { get; set; }

    public int? ProducerId { get; set; }

    public string Name { get; set; } = default!;

    public ProductType? Type { get; set; } = ProductType.Good;
    
    public SaleModel SaleModel  { get; set; } = SaleModel.Unit;
    
    public bool AllowsQuantity => SaleModel is SaleModel.Unit;
    
    public bool IsRecurring => SaleModel is SaleModel.Subscription;

    public ProductCategory? Category { get; internal set; }

    public int? CategoryId { get; private set; }

    public string Description { get; set; } = default!;

    public string? Headline { get; set; }

    public string? Sku { get; set; }

    public string? Gtin { get; set; }

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
    public PricingModel PricingModel { get; set; }
    
    public DiscountApplicationMode DiscountApplicationMode { get; set; }

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

    public decimal? PurchasePrice { get; set; }

    public ProductImage? Image { get; set; }

    public string? ImageId { get; set; }

    public IReadOnlyCollection<ProductImage> Images => _images;

    public void AddImage(ProductImage productImage) => _images.Add(productImage);

    public void RemoveImage(ProductImage productImage) => _images.Remove(productImage);

    public string Handle { get; set; } = default!;
    

    public bool? HasOptions { get; set; } = false;
    
    public ProductStructure Structure { get; set; } = ProductStructure.Single;
    
    public bool IsBuyable => Structure is ProductStructure.Single or ProductStructure.IsVariant;
    
    public bool RequiresVariantSelection => Structure is ProductStructure.WithVariants;
    public bool HasVariants => Structure is ProductStructure.WithVariants;
    public bool IsVariant => Structure is ProductStructure.IsVariant;
    
    public Product? Parent { get; internal set; }

    public int? ParentId { get; internal set; }

    public IReadOnlyCollection<ProductAttribute> ProductAttributes => _productAttributes;

    public IReadOnlyCollection<AttributeGroup> AttributeGroups => _attributeGroups;

    public IReadOnlyCollection<Product> Variants => _variants;

    public IReadOnlyCollection<Option> Options => _options;

    public IReadOnlyCollection<ProductOption> ProductOptions => _productOptions;

    public IReadOnlyCollection<OptionGroup> OptionGroups => _optionGroups;

    public ProductListingState ListingState { get; set; }

    public IReadOnlyCollection<ProductVariantOption> ProductVariantOptions => _productVariantOptions;

    public bool AddVariant(Product variant)
    {
        if (Structure is not ProductStructure.WithVariants)
            throw new InvalidOperationException("The product doesn't support variants.");
        
        if (variant.SaleModel != SaleModel)
            throw new InvalidOperationException("All variants must have the same SaleModel as the parent product.");
        
        if (_variants.Add(variant))
        {
            variant.OrganizationId = OrganizationId;
            variant.Store = Store;
            variant.Category = Category;
            variant.Parent = this;
            variant.Structure = ProductStructure.IsVariant;
            return true;
        }
        return false;
    }

    public bool RemoveVariant(Product variant, bool detachVariant = false)
    {
        if (_variants.Remove(variant))
        {
            if (detachVariant)
            {
                variant.Parent = null;
                variant.Structure = ProductStructure.Single;

                if (!_variants.Any())
                {
                    Structure = ProductStructure.Single;
                }
            }

            return true;
        }

        return false;
    }

    public bool AddProductAttribute(ProductAttribute productAttribute)
    {
        productAttribute.OrganizationId = OrganizationId;
        return _productAttributes.Add(productAttribute);
    }

    public bool RemoveProductAttribute(ProductAttribute productAttribute)
    {
        return _productAttributes.Remove(productAttribute);
    }

    public bool AddOptionGroup(OptionGroup group)
    {
        if (_optionGroups.Add(group))
        {
            group.OrganizationId = OrganizationId;
            return true;
        }

        return false;
    }

    public bool RemoveOptionGroup(OptionGroup group)
    {
        return _optionGroups.Remove(group);
    }

    public bool AddOption(Option option, bool isInherited = false)
    {
        option.OrganizationId = OrganizationId;

        var productOption = new ProductOption()
        {
            OrganizationId = OrganizationId,
            Product = this,
            Option = option,
            IsInherited = isInherited
        };
        return _productOptions.Add(productOption);

        //option.OrganizationId = OrganizationId;
        //return _options.Add(option);
    }

    public bool RemoveOption(Option option)
    {
        return _options.Remove(option);
    }

    public bool RemoveOption(ProductOption option)
    {
        return _productOptions.Remove(option);
    }

    public (decimal price, decimal? regularPrice) GetTotalOptionsPrice()
    {
        var price = 0m;
        var regularPrice = 0m;

        foreach (var productOption in ProductOptions)
        {
            var option = productOption.Option;

            if (option is SelectableOption selectableOption)
            {
                var isSelected = selectableOption.IsSelected;

                if (!isSelected)
                {
                    continue;
                }

                if (isSelected)
                {
                    price += selectableOption.Price.GetValueOrDefault();
                    regularPrice += selectableOption.Price.GetValueOrDefault();
                }
            }
            else if (option is ChoiceOption { DefaultValue: not null } choiceOption)
            {
                var value = choiceOption.DefaultValue;

                price += value.Price.GetValueOrDefault();
                regularPrice += value.Price.GetValueOrDefault();
            }
            else if (option is NumericalValueOption numericalValueOption)
            {
                price += numericalValueOption.Price.GetValueOrDefault() * numericalValueOption.DefaultNumericalValue.GetValueOrDefault();
                regularPrice += numericalValueOption.Price.GetValueOrDefault() * numericalValueOption.DefaultNumericalValue.GetValueOrDefault();
            }
        }

        return (price, regularPrice);
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

    public IReadOnlyCollection<ProductSubscriptionPlan> SubscriptionPlans => _subscriptionPlans;

    // Add or remove subscription plans
    public bool AddSubscriptionPlan(ProductSubscriptionPlan subscriptionPlan) => _subscriptionPlans.Add(subscriptionPlan);
    public void RemoveSubscriptionPlan(ProductSubscriptionPlan subscriptionPlan) => _subscriptionPlans.Remove(subscriptionPlan);

    // Calculate final price based on subscription
    public decimal GetSubscriptionPrice(bool isSubscribed)
    {
        if (isSubscribed && _subscriptionPlans.Any())
            return _subscriptionPlans.Min(plan => plan.GetSubscriptionPrice(Price));

        return Price;
    }

    public decimal GetSavingsWithSubscription(decimal basePrice)
    {
        return basePrice - GetSubscriptionPrice(true);
    }
}

public enum PricingModel
{
    FixedPrice,          // Använd Price rakt av, inga options.
    OptionsBased        // Price + eventuella optionskostnader.
}

public enum DiscountApplicationMode
{
    OnBasePrice,               // Rabatt gäller BasePrice innan options läggs på.
    AfterOptions               // Rabatt gäller på totalsumman (BasePrice + options).
}

public enum ProductStructure
{
    Single,     // Ingen variant – säljs som den är
    WithVariants, // Har varianter – ej direkt köpbar
    IsVariant    // En variant av en annan produkt
}

public enum SaleModel
{
    Unit,           // Buy by quantity (e.g., 5 items or 3 sessions)
    Subscription,   // Recurring purchase
    OneTime         // Single-use, no quantity (e.g., activation fee)
}