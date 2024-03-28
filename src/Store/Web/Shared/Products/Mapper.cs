namespace BlazorApp.Products;

using BlazorApp.ProductCategories;
using BlazorApp.Brands;

public static class Mapper
{
    public static Product Map(this YourBrand.StoreFront.Product product)
        => new(product.Id!, product.Name!, product.Brand?.Map(), product.Category?.ToParentDto2(), product.Image?.Map()!, product.Images.Select(x => x.Map()), product.Description!, product.Price, product.VatRate, product.RegularPrice, product.DiscountRate, product.Handle, product.HasVariants, product.Attributes.Select(x => x.Map()), product.Options.Select(x => x.Map()));

    public static ProductImage Map(this YourBrand.StoreFront.ProductImage image) => new(image.Id, image.Title, image.Text, image.Url);

    public static ProductAttribute Map(this YourBrand.StoreFront.ProductAttribute attribute) => new(attribute.Attribute.Map(), attribute.Value?.Map(), attribute.ForVariant, attribute.IsMainAttribute);

    public static Attribute Map(this YourBrand.StoreFront.Attribute attribute) => new(attribute.Id, attribute.Name, attribute.Description, attribute.Group?.Map(), attribute.Values.Select(x => x.Map()).ToList());

    public static AttributeGroup Map(this YourBrand.StoreFront.AttributeGroup group) => new(group.Id, group.Name, group.Description);

    public static AttributeValue Map(this YourBrand.StoreFront.AttributeValue value) => new(value.Id, value.Name, value.Seq);

    public static ProductOption Map(this YourBrand.StoreFront.ProductOption option) => new(option.Option.Map(), option.IsInherited);

    public static Option Map(this YourBrand.StoreFront.Option option) => new(option.Id, option.Name, option.Description, (OptionType)option.OptionType, option.Group?.Map(), option.IsRequired, option.Sku, option.Price, option.IsSelected, option.Values.Select(x => x.Map()).ToList(), option.DefaultValue?.Map(), option.MinNumericalValue, option.MaxNumericalValue, option.DefaultNumericalValue, option.TextValueMinLength, option.TextValueMaxLength, option.DefaultTextValue);

    public static OptionGroup Map(this YourBrand.StoreFront.OptionGroup group) => new(group.Id, group.Name, group.Description, group.Seq, group.Min, group.Max);

    public static OptionValue Map(this YourBrand.StoreFront.OptionValue value) => new(value.Id, value.Name, value.Sku, value.Price, value.Seq);
}