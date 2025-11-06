namespace BlazorApp.Products;

using BlazorApp.Brands;
using BlazorApp.ProductCategories;
using ProductPriceTierType = YourBrand.StoreFront.ProductPriceTierType;

public sealed record Product(long Id, string Name, Brand? brand, ProductCategoryParent? Category, ProductImage? Image, IEnumerable<ProductImage> Images, string Description, decimal Price, double? VatRate, decimal? RegularPrice, double? DiscountRate, string Handle, bool HasVariants, IEnumerable<ProductAttribute> Attributes, IEnumerable<ProductOption> Options);

public sealed record ProductSubscriptionPlan(string Id, string Name, decimal Price, ProductSubscriptionPlanTrial? Trial);

public sealed record ProductSubscriptionPlanTrial(int PeriodLength, decimal Price);

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

public static class Mappings
{
    public static ProductSubscriptionPlan Map(this YourBrand.StoreFront.ProductSubscriptionPlan subscriptionPlan)
    {
        return new ProductSubscriptionPlan(
            subscriptionPlan.Id,
            subscriptionPlan.Name,
            subscriptionPlan.Price,
            subscriptionPlan.Trial is not null ? subscriptionPlan.Trial.ToDto() : null);
    }

    public static ProductSubscriptionPlanTrial ToDto(this YourBrand.StoreFront.ProductSubscriptionPlanTrial trialPeriod)
    {
        return new ProductSubscriptionPlanTrial(trialPeriod.PeriodLength, trialPeriod.Price);
    }
}

public record class CalculateProductPriceRequest(
    List<ProductOptionValue> OptionValues,
    int Quantity = 1,
    string? SubscriptionPlanId = null);

public record ProductOptionValue(string OptionId, decimal? NumericValue, string? ChoiceValueId);

public record ProductPriceResult(
    decimal BasePrice,
    decimal OptionsTotal,
    int Quantity,
    decimal UnitPrice,
    decimal TierDiscount,
    decimal ProductDiscount,
    decimal SubscriptionDiscount,
    decimal Total,
    ProductPriceTierApplication? PriceTier)
{
    public decimal DiscountAmount => TierDiscount + ProductDiscount + SubscriptionDiscount;
}

public record ProductPriceTierApplication(
    string Id,
    int FromQuantity,
    int? ToQuantity,
    ProductPriceTierType TierType,
    decimal Value,
    int DiscountedUnits,
    decimal EffectiveUnitPrice);

public static class Mappings2
{
    public static YourBrand.StoreFront.CalculateProductPriceRequest Map(this CalculateProductPriceRequest request)
    {
        return new YourBrand.StoreFront.CalculateProductPriceRequest {
            OptionValues = request.OptionValues.Select(x => x.ToDto()).ToList(),
            SubscriptionPlanId =  request.SubscriptionPlanId,
            Quantity = request.Quantity
        };
    }

    public static YourBrand.StoreFront.ProductOptionValue ToDto(this ProductOptionValue productOptionValue)
    {
        return new YourBrand.StoreFront.ProductOptionValue
        {
            OptionId = productOptionValue.OptionId, 
            NumericValue = productOptionValue.NumericValue, 
            ChoiceValueId = productOptionValue.ChoiceValueId
        };
    }
    
    public static ProductPriceResult Map(this YourBrand.StoreFront.ProductPriceResult productPriceResult)
    {
        return new ProductPriceResult(
            productPriceResult.BasePrice,
            productPriceResult.OptionsTotal,
            productPriceResult.Quantity,
            productPriceResult.UnitPrice,
            productPriceResult.TierDiscount,
            productPriceResult.ProductDiscount,
            productPriceResult.SubscriptionDiscount,
            productPriceResult.Total,
            productPriceResult.PriceTier?.ToDto());
    }

    public static ProductPriceTierApplication ToDto(this YourBrand.StoreFront.ProductPriceTierApplication priceTier)
    {
        return new ProductPriceTierApplication(
            priceTier.Id!,
            priceTier.FromQuantity,
            priceTier.ToQuantity,
            priceTier.TierType,
            priceTier.Value,
            priceTier.DiscountedUnits,
            priceTier.EffectiveUnitPrice);
    }
}
