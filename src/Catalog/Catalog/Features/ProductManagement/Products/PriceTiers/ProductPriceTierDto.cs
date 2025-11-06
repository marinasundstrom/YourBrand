using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Features.ProductManagement.Products.PriceTiers;

public sealed record ProductPriceTierDto(
    string Id,
    int FromQuantity,
    int? ToQuantity,
    ProductPriceTierType TierType,
    decimal Value);

public static class ProductPriceTierMappings
{
    public static ProductPriceTierDto ToDto(this ProductPriceTier tier)
    {
        return new ProductPriceTierDto(
            tier.Id,
            tier.FromQuantity,
            tier.ToQuantity,
            tier.TierType,
            tier.Value);
    }
}
