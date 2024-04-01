using Core;

using YourBrand.Catalog.Domain.Enums;

namespace YourBrand.Catalog.Domain.Entities;

public sealed class Product
{
    readonly HashSet<ProductAttribute> _productAttributes = new HashSet<ProductAttribute>();

    readonly HashSet<AttributeGroup> _attributeGroups = new HashSet<AttributeGroup>();

    readonly HashSet<Product> _variants = new HashSet<Product>();

    readonly HashSet<Option> _options = new HashSet<Option>();

    readonly HashSet<ProductOption> _productOptions = new HashSet<ProductOption>();

    readonly HashSet<OptionGroup> _optionGroups = new HashSet<OptionGroup>();

    readonly HashSet<ProductVariantOption> _productVariantOptions = new HashSet<ProductVariantOption>();

    readonly HashSet<ProductImage> _images = new HashSet<ProductImage>();
    private decimal _price;
    private decimal? _regularPrice;

    public Product() { }

    public Product(string name, string handle)
    {
        Name = name;
        Handle = handle;
    }

    public long Id { get; private set; }

    public Store? Store { get; internal set; }

    public string? StoreId { get; private set; }

    public Brand? Brand { get; set; }

    public int? BrandId { get; set; }

    public string Name { get; set; } = default!;

    public ProductCategory? Category { get; internal set; }

    public long? CategoryId { get; private set; }


    public string Description { get; set; } = default!;

    public string? Headline { get; set; }

    public string? Sku { get; set; }

    public string? Gtin { get; set; }

    public decimal Price
    {
        get => _price;
        internal set
        {
            if (RegularPrice is not null && value >= RegularPrice.GetValueOrDefault())
            {
                throw new Exception("Price can not be greater than or equal to Regular Price");
            }

            _price = value;
        }
    }

    //public decimal Vat { get; set; }

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
            {
                throw new Exception("Regular Price can not be less than or equal to Price");
            }

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

    public bool? IsCustomizable { get; set; } = false;

    public bool HasVariants { get; set; } = false;

    public Product? Parent { get; internal set; }

    public long? ParentId { get; internal set; }

    public IReadOnlyCollection<ProductAttribute> ProductAttributes => _productAttributes;

    public IReadOnlyCollection<AttributeGroup> AttributeGroups => _attributeGroups;

    public IReadOnlyCollection<Product> Variants => _variants;

    public IReadOnlyCollection<Option> Options => _options;

    public IReadOnlyCollection<ProductOption> ProductOptions => _productOptions;

    public IReadOnlyCollection<OptionGroup> OptionGroups => _optionGroups;

    public ProductListingState ListingState { get; set; }

    public IReadOnlyCollection<ProductVariantOption> ProductVariantOptions => _productVariantOptions;

    public void AddVariant(Product variant)
    {
        _variants.Add(variant);
        variant.Store = this.Store;
        variant.Category = this.Category;
        variant.Parent = this;
    }

    public void RemoveVariant(Product variant)
    {
        _variants.Add(variant);
    }

    public void AddProductOption(ProductOption productOption)
    {
        _productOptions.Add(productOption);
    }

    public void RemoveProductOption(ProductOption option)
    {
        _productOptions.Remove(option);
    }

    public void AddProductAttribute(ProductAttribute productAttribute)
    {
        _productAttributes.Add(productAttribute);
    }

    public void RemoveProductAttribute(ProductAttribute productAttribute)
    {
        _productAttributes.Remove(productAttribute);
    }

    public void AddOptionGroup(OptionGroup group)
    {
        _optionGroups.Add(group);
    }

    public void RemoveOptionGroup(OptionGroup group)
    {
        _optionGroups.Remove(group);
    }

    public void AddOption(Option option)
    {
        _options.Add(option);
    }

    public void RemoveOption(Option option)
    {
        _options.Remove(option);
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
        var originalPrice = Price;

        Price = discountPrice;
        RegularPrice = originalPrice;
        DiscountRate = PriceCalculations.CalculateDiscountRate(Price, RegularPrice.GetValueOrDefault());
        Discount = RegularPrice - Price;
    }

    public void RestoreRegularPrice()
    {
        var regularPrice = RegularPrice.GetValueOrDefault();

        RegularPrice = null;
        Price = regularPrice;
        DiscountRate = null;
        Discount = null;
    }
}