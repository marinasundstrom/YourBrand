namespace BlazorApp.Products;

public class ProductViewModel
{
    private IProductsService productsService;
    private Product? product;
    private Product? variant;
    private IEnumerable<ProductOption>? productOptions;
    private IEnumerable<ProductAttribute>? productAttributes;

    public ProductViewModel(IProductsService productsService)
    {
        this.productsService = productsService;
    }

    public ProductViewModel()
    {
    }

    public void SetClient(IProductsService productsService) => this.productsService = productsService;

    public long ProductId => Product?.Id ?? 0;

    public string Id => Product?.Handle ?? string.Empty;

    public string VariantId => Variant?.Handle ?? string.Empty;

    public string Name => Product?.Name ?? string.Empty;

    //public string Headline => Variant?.Headline ?? Product?.Headline ?? string.Empty;

    public string Description => Variant?.Description ?? Product?.Description ?? string.Empty;

    string? _image;

    public string Image
    {
        get => _image ?? Variant?.Image?.Url ?? Product?.Image?.Url ?? string.Empty;
        set => _image = value;
    }

    //public string Currency => (Variant?.Price ?? Product!.Price)!.Currency;

    public decimal Price => Variant?.Price ?? Product?.Price ?? 0; //Variant?.Price?.Amount ?? Product?.Price?.Amount ?? 0;

    async Task Foo()
    {
        await this.productsService.CalculatePrice(Product!.Handle, new CalculateProductPriceRequest([], null));
    }
    
    public decimal Total => Price
                + OptionGroups.SelectMany(x => x.Options)
                // Exclude options with default values
                .Where(x => x.IsSelected || x.SelectedValueId is not null)
                .Select(x => x.Price.GetValueOrDefault() + (x.Values.FirstOrDefault(x3 => x3.Id == x?.SelectedValueId)?.Price ?? 0))
                .Sum()
                 + OptionGroups.SelectMany(x => x.Options)
                .Where(x => x.OptionType == OptionType.NumericalValue)
                .Select(x => x.Price.GetValueOrDefault() * x.NumericalValue.GetValueOrDefault())
                .Sum();

    public decimal? RegularPrice => Product?.RegularPrice; // ?.Amount;

    //public int? Available => Variant?.Available ?? Product?.Available;

    public async Task Initialize(string id, string? variantId)
    {
        Product = await productsService.GetProductById(id);

        await Load();

        var hasVariants = Product.HasVariants;

        if (hasVariants)
        {
            Product? variant;

            if (variantId is not null)
            {
                variant = await productsService.GetProductById(variantId);
            }
            else
            {
                var variants = (await productsService.GetProductVariants(Id, 1, 1, null)).Items;
                variant = await productsService.GetProductById(variants.First().Id.ToString());
            }

            AttributeGroups.ForEach(x => x.Attributes.ForEach(x => x.SelectedValueId = null));

            var attrs = AttributeGroups.SelectMany(x => x.Attributes);

            foreach (var attr in variant.Attributes.Where(x => x.ForVariant))
            {
                var x = attrs.FirstOrDefault(x => x.Id == attr.Attribute.Id);
                if (x is not null)
                {
                    x.SelectedValueId = attr.Value?.Id;
                }
            }

            var selectedAttributeValues = AttributeGroups
                .SelectMany(x => x.Attributes)
                .Where(x => x.ForVariant)
                .Where(x => !x.IsMainAttribute)
                .Where(x => x.SelectedValueId is not null)
                .ToDictionary(x => x.Id, x => x.SelectedValueId);

            Variants.AddRange(
                await productsService.FindProductVariantsByAttributes(Id, selectedAttributeValues));

            await SelectVariant(variant);
        }

        Updated?.Invoke(this, EventArgs.Empty);
    }

    private async Task Load()
    {
        productOptions = Variant?.Options ?? Product!.Options;
        productAttributes = Variant?.Attributes ?? Product!.Attributes;

        CreateOptionsVM();
        CreateAttributesVM();
    }

    public List<AttributeGroupVM> AttributeGroups { get; set; } = new List<AttributeGroupVM>();

    public List<OptionGroupVM> OptionGroups { get; set; } = new List<OptionGroupVM>();

    public List<Product> Variants { get; set; } = new List<Product>();

    public async Task SelectVariant(Product variant)
    {
        if (this.variant?.Id == variant?.Id) return;

        this.variant = variant;

        await Load();

        var attributes = AttributeGroups.SelectMany(x => x.Attributes);

        foreach (var attr in variant.Attributes.Where(x => x.ForVariant))
        {
            var selectedAttr = attributes.FirstOrDefault(x => x.Id == attr?.Attribute?.Id);
            if (selectedAttr is not null)
            {
                selectedAttr.SelectedValueId = selectedAttr.Values.FirstOrDefault(x => x.Id == attr.Value?.Id)?.Id;
            }
        }

        foreach (var attr in AttributeGroups.SelectMany(x => x.Attributes))
        {
            await UpdateAttr(attr);
        }

        Updated?.Invoke(this, EventArgs.Empty);
    }

    public async Task UpdateVariant()
    {
        var selectedAttributeValues = AttributeGroups
            .SelectMany(x => x.Attributes)
            .Where(x => x.ForVariant)
            .Where(x => !x.IsMainAttribute)
            .Where(x => x.SelectedValueId is not null)
            .ToDictionary(x => x.Id, x => x.SelectedValueId);

        var products = await productsService.FindProductVariantsByAttributes(Id, selectedAttributeValues.ToDictionary(x => x.Key, x => x.Value!));

        Variants.Clear();
        Variants.AddRange(products);

        var selectedAttributes = AttributeGroups
           .SelectMany(x => x.Attributes)
           .Where(x => x.ForVariant)
           .Where(x => x.SelectedValueId is not null);

        variant = await productsService.FindProductVariantByAttributes(Id, selectedAttributes.ToDictionary(x => x.Id, x => x.SelectedValueId!));

        foreach (var attr in AttributeGroups.SelectMany(x => x.Attributes))
        {
            await UpdateAttr(attr);
        }

        await Load();
    }

    private async Task UpdateAttr(AttributeVM attribute)
    {
        foreach (var value in attribute.Values)
        {
            value.Disabled = true;
        }

        var selectedAttributeValues = AttributeGroups
            .SelectMany(x => x.Attributes)
            .Where(x => x.ForVariant)
            //.Where(x => !x.IsMainAttribute)
            .Where(x => x.Id != attribute.Id)
            .Where(x => x.SelectedValueId is not null)
            .ToDictionary(x => x.Id, x => x.SelectedValueId);

        var results = await productsService.GetAvailableProductVariantAttributesValues(Id, attribute.Id, selectedAttributeValues);

        foreach (var result in results)
        {
            var value = attribute.Values.FirstOrDefault(x => x.Id == result.Id);
            value!.Disabled = false;
        }
    }

    private void CreateOptionsVM()
    {
        var groups = productOptions
            .Select(x => x.Option)
            .Select(x => x.Group ?? new OptionGroup(null!, string.Empty, string.Empty, null, null, null))
            .DistinctBy(x => x.Id);

        foreach (var optionGroup in groups)
        {
            var group = OptionGroups.FirstOrDefault(x => x.Id == optionGroup.Id);
            if (group is null)
            {
                group = new OptionGroupVM()
                {
                    Id = optionGroup.Id,
                    Name = optionGroup.Name,
                    Description = optionGroup.Description!,
                    Min = optionGroup.Min,
                    Max = optionGroup.Max
                };

                OptionGroups.Add(group);
            }

            foreach (var option in productOptions.Select(x => x.Option).Where(x => x.Group?.Id == group.Id))
            {
                var o = group.Options.FirstOrDefault(x => x.Id == option.Id);
                if (o is null)
                {
                    o = new OptionVM
                    {
                        Id = option.Id,
                        Name = option.Name,
                        Description = option.Description!,
                        Group = option.Group!,
                        OptionType = option.OptionType,
                        Price = option.Price,
                        ProductId = option.Sku,
                        IsSelected = option.IsSelected.GetValueOrDefault(),
                        SelectedValueId = option.DefaultValue?.Id,
                        MinNumericalValue = option.MinNumericalValue,
                        MaxNumericalValue = option.MaxNumericalValue,
                        NumericalValue = option.DefaultNumericalValue,
                        TextValue = option.DefaultTextValue,
                        TextValueMaxLength = option.TextValueMaxLength,
                        TextValueMinLength = option.TextValueMinLength
                    };

                    o.Values.AddRange(option.Values.Select(x => new OptionValueVM
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price
                    }));

                    group.Options.Add(o);
                }
            }

            foreach (var option in group.Options.ToList())
            {
                var o1 = productOptions.FirstOrDefault(x => x.Option.Id == option.Id);
                if (o1 is null)
                {
                    group.Options.Remove(option);
                }
            }
        }

        foreach (var group in OptionGroups.ToList())
        {
            var o1 = groups.FirstOrDefault(x => x.Id == group.Id);
            if (o1 is null)
            {
                OptionGroups.Remove(group);
            }
        }
    }

    private void CreateAttributesVM()
    {
        var groups = productAttributes
            .Select(x => x.Attribute.Group ?? new AttributeGroup(null!, string.Empty, string.Empty))
            .DistinctBy(x => x.Id);

        foreach (var attributeGroup in groups)
        {
            var group = AttributeGroups.FirstOrDefault(x => x.Id == attributeGroup.Id);
            if (group is null)
            {
                group = new AttributeGroupVM()
                {
                    Id = attributeGroup.Id,
                    Name = attributeGroup.Name
                };

                AttributeGroups.Add(group);
            }

            foreach (var attribute in productAttributes.Where(x => x.Attribute.Group?.Id == group.Id))
            {
                var attr = group.Attributes.FirstOrDefault(x => x.Id == attribute.Attribute.Id);
                if (attr is null)
                {
                    attr = new AttributeVM
                    {
                        Id = attribute.Attribute.Id,
                        Name = attribute.Attribute.Name,
                        ForVariant = attribute.ForVariant,
                        IsMainAttribute = attribute.IsMainAttribute
                    };

                    attr.Values.AddRange(attribute.Attribute.Values.Select(x => new AttributeValueVM
                    {
                        Id = x.Id,
                        Name = x.Name
                    }));

                    attr.SelectedValueId = attr.Values.FirstOrDefault()?.Id;

                    group.Attributes.Add(attr);
                }
            }

            foreach (var attr in group.Attributes.ToList())
            {
                var a = productAttributes.FirstOrDefault(x => x.Attribute.Id == attr.Id);
                if (a is null)
                {
                    group.Attributes.Remove(attr);
                }
            }
        }

        foreach (var group in AttributeGroups.ToList())
        {
            var o1 = groups.FirstOrDefault(x => x.Id == group.Id);
            if (o1 is null)
            {
                AttributeGroups.Remove(group);
            }
        }
    }

    public event EventHandler Updated;

    public Product? Product { get => product; set => product = value; }

    public Product? Variant { get => variant; set => variant = value; }

    public void LoadData(IEnumerable<Option> options)
    {
        foreach (var option in options)
        {
            var o = OptionGroups
                .SelectMany(x => x.Options)
                .FirstOrDefault(x => x.Id == option.Id);

            if (o is not null)
            {
                o.IsSelected = option.IsSelected.GetValueOrDefault();
                o.SelectedValueId = option.SelectedValueId;
                o.NumericalValue = option.NumericalValue;
                o.TextValue = option.TextValue;
            }
        }
    }

    public IEnumerable<Option> GetData()
    {
        return OptionGroups.SelectMany(x => x.Options).Select(x =>
        {
            return new Option
            {
                Id = x.Id,
                Name = x.Name,
                OptionType = (int)x.OptionType,
                ProductId = x.ProductId,
                Price = x.Price,
                IsSelected = x.IsSelected,
                SelectedValueId = x.SelectedValueId,
                NumericalValue = x.NumericalValue,
                TextValue = x.TextValue
            };
        });
    }

    public class Option
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public int OptionType { get; set; }

        public string? ProductId { get; set; }

        public decimal? Price { get; set; }

        public string? TextValue { get; set; }

        public int? NumericalValue { get; set; }

        public bool? IsSelected { get; set; }

        public string? SelectedValueId { get; set; }
    }

    public void SelectImage(ProductImage productImage)
    {
        Image = productImage.Url;
    }
}