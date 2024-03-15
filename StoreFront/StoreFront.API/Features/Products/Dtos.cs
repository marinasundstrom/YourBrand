namespace YourBrand.StoreFront.API.Features.Products;

using YourBrand.StoreFront.API.Features.Products.Categories;

public sealed record Product(long Id, string Name, Brand? Brand, ProductCategoryParent? Category, ProductImage? Image, IEnumerable<ProductImage> Images, string Description, decimal Price, double? VatRate, decimal? RegularPrice, double? DiscountRate, string Handle, bool HasVariants, IEnumerable<ProductAttribute> Attributes, IEnumerable<ProductOption> Options);

public sealed record ProductImage(string Id, string? Title, string? Text, string Url);

public sealed record ProductAttribute(Attribute Attribute, AttributeValue? Value, bool ForVariant, bool IsMainAttribute);

public sealed record Attribute(string Id, string Name, string? Description, AttributeGroup? Group, ICollection<AttributeValue> Values);

public sealed record AttributeGroup(string Id, string Name, string? Description);

public sealed record AttributeValue(string Id, string Name, int? Seq);

public sealed record ProductOption(Option Option, bool IsInherited);

public sealed record Option(string Id, string Name, string? Description, OptionType OptionType, OptionGroup? Group, bool IsRequired, string? Sku, decimal? Price, bool? IsSelected,
    ICollection<OptionValue> Values, OptionValue? DefaultValue,
    int? MinNumericalValue, int? MaxNumericalValue, int? DefaultNumericalValue, int? TextValueMinLength, int? TextValueMaxLength, string? DefaultTextValue);

public sealed record OptionGroup(string Id, string Name, string? Description, int? Seq, int? Min, int? Max);

public enum OptionType
{
    YesOrNo = 0,

    Choice = 1,

    NumericalValue = 2,

    TextValue = 3,
}

public sealed record OptionValue(string Id, string Name, string? Sku, decimal? Price, int? Seq);

public static class Mapper
{
    public static Brand Map(this YourBrand.Catalog.Brand brand)
        => new(brand.Id!, brand.Name!);

    public static Product Map(this YourBrand.Catalog.Product product)
        => new(product.Id!, product.Name!, product.Brand?.Map(), product.Category?.ToParentDto3(), product.Image?.Map(), product.Images.Select(x => x.Map()), product.Description!, product.Price, product.VatRate, product.RegularPrice, product.DiscountRate, product.Handle, product.HasVariants, product.Attributes.Select(x => x.Map()), product.Options.Select(x => x.Map()));

    public static ProductAttribute Map(this YourBrand.Catalog.ProductAttribute attribute) => new(attribute.Attribute.Map(), attribute.Value?.Map(), attribute.ForVariant, attribute.IsMainAttribute);

    public static ProductImage Map(this YourBrand.Catalog.ProductImage image) => new(image.Id, image.Title, image.Text, image.Url);

    public static Attribute Map(this YourBrand.Catalog.Attribute attribute) => new(attribute.Id, attribute.Name, attribute.Description, attribute.Group?.Map(), attribute.Values.Select(x => x.Map()).ToList());

    public static AttributeGroup Map(this YourBrand.Catalog.AttributeGroup group) => new(group.Id, group.Name, group.Description);

    public static AttributeValue Map(this YourBrand.Catalog.AttributeValue value) => new(value.Id, value.Name, value.Seq);

    public static ProductOption Map(this YourBrand.Catalog.ProductOption option) => new(option.Option.Map(), option.IsInherited);

    public static Option Map(this YourBrand.Catalog.Option option) => new(option.Id, option.Name, option.Description, (OptionType)option.OptionType, option.Group?.Map(), option.IsRequired, option.Sku, option.Price, option.IsSelected, option.Values.Select(x => x.Map()).ToList(), option.DefaultValue?.Map(), option.MinNumericalValue, option.MaxNumericalValue, option.DefaultNumericalValue, option.TextValueMinLength, option.TextValueMaxLength, option.DefaultTextValue);

    public static OptionGroup Map(this YourBrand.Catalog.OptionGroup group) => new(group.Id, group.Name, group.Description, group.Seq, group.Min, group.Max);

    public static OptionValue Map(this YourBrand.Catalog.OptionValue value) => new(value.Id, value.Name, value.Sku, value.Price, value.Seq);
}