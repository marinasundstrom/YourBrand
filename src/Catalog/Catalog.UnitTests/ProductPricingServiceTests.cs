using System;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Features.ProductManagement;
using Xunit;

namespace YourBrand.Catalog.UnitTests;

public class ProductPricingServiceTests
{
    private readonly ProductPricingService _service = new();

    [Fact]
    public void CalculatePrice_WithNoTiers_UsesBasePrice()
    {
        var product = CreateProduct(100m);

        var result = _service.CalculatePrice(product, Array.Empty<ProductOptionValue>(), null, quantity: 3);

        Assert.Equal(100m, result.BasePrice);
        Assert.Equal(3, result.Quantity);
        Assert.Equal(0m, result.TierDiscount);
        Assert.Equal(300m, result.Total);
        Assert.Equal(100m, result.UnitPrice);
    }

    [Fact]
    public void CalculatePrice_AppliesPricePerUnitTier()
    {
        var product = CreateProduct(120m);
        var price = product.CurrentPrice;
        price.AddPriceTier(new ProductPriceTier(price.Id, 5, null, ProductPriceTierType.PricePerUnit, 90m));

        var result = _service.CalculatePrice(product, Array.Empty<ProductOptionValue>(), null, quantity: 5);

        Assert.Equal(5, result.Quantity);
        Assert.Equal(120m * 5 - 90m * 5, result.TierDiscount);
        Assert.Equal(90m, result.UnitPrice);
        Assert.Equal(450m, result.Total);
    }

    [Fact]
    public void CalculatePrice_AppliesDiscountPerUnitTier()
    {
        var product = CreateProduct(80m);
        var price = product.CurrentPrice;
        price.AddPriceTier(new ProductPriceTier(price.Id, 3, 10, ProductPriceTierType.DiscountPerUnit, 15m));

        var result = _service.CalculatePrice(product, Array.Empty<ProductOptionValue>(), null, quantity: 4);

        Assert.Equal(4, result.Quantity);
        Assert.Equal(15m * 4, result.TierDiscount);
        Assert.Equal(80m * 4 - 15m * 4, result.Total);
    }

    [Fact]
    public void CalculatePrice_AppliesDiscountPerAdditionalUnitTier()
    {
        var product = CreateProduct(50m);
        var price = product.CurrentPrice;
        price.AddPriceTier(new ProductPriceTier(price.Id, 2, null, ProductPriceTierType.DiscountPerAdditionalUnit, 10m));

        var result = _service.CalculatePrice(product, Array.Empty<ProductOptionValue>(), null, quantity: 5);

        Assert.Equal(5, result.Quantity);
        Assert.Equal(10m * 4, result.TierDiscount);
        Assert.Equal(50m * 5 - 40m, result.Total);
    }

    [Fact]
    public void CalculatePrice_RespectsAdditionalUnitUpperBound()
    {
        var product = CreateProduct(60m);
        var price = product.CurrentPrice;
        price.AddPriceTier(new ProductPriceTier(price.Id, 2, 4, ProductPriceTierType.DiscountPerAdditionalUnit, 5m));

        var result = _service.CalculatePrice(product, Array.Empty<ProductOptionValue>(), null, quantity: 6);

        Assert.Equal(6, result.Quantity);
        Assert.Equal(5m * 3, result.TierDiscount);
        Assert.Equal(60m * 6 - 15m, result.Total);
    }

    private static Product CreateProduct(decimal price)
    {
        var product = new Product("Test", Guid.NewGuid().ToString())
        {
            PricingModel = PricingModel.FixedPrice,
            DiscountApplicationMode = DiscountApplicationMode.OnBasePrice
        };

        product.SetPrice(price);

        return product;
    }
}
