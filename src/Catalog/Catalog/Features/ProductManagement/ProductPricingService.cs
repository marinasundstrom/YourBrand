using System.Linq;

using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Features.ProductManagement;

public class ProductPricingService
{
    public ProductPriceResult CalculatePrice(
        Product product,
        IEnumerable<ProductOptionValue> selectedOptionValues,
        ProductSubscriptionPlan? subscriptionPlan = null,
        int quantity = 1)
    {
        ArgumentNullException.ThrowIfNull(product);

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        var p = product.Prices.First();

        var basePrice = p.Price;
        var optionDetails = new List<ProductOptionPriceDetail>();

        var optionsTotal = CalculateOptionsTotal(product, selectedOptionValues, optionDetails);

        var optionsSubtotal = product.PricingModel == PricingModel.OptionsBased ? optionsTotal * quantity : 0m;

        var tierResult = CalculateTierAdjustment(p, quantity);

        decimal subtotal = tierResult.SubtotalAfterTier;

        if (product.PricingModel == PricingModel.OptionsBased)
        {
            subtotal += optionsSubtotal;
        }

        decimal productDiscount = 0;

        if (p.DiscountRate.HasValue)
        {
            if (product.DiscountApplicationMode == DiscountApplicationMode.OnBasePrice)
            {
                var discountedBase = ApplyDiscount(tierResult.SubtotalAfterTier, p.DiscountRate.Value);
                productDiscount = tierResult.SubtotalAfterTier - discountedBase;
                subtotal = discountedBase + optionsSubtotal;
            }
            else if (product.DiscountApplicationMode == DiscountApplicationMode.AfterOptions)
            {
                productDiscount = subtotal * (decimal)p.DiscountRate.Value / 100;
                subtotal -= productDiscount;
            }
        }

        decimal subscriptionDiscount = 0;

        if (subscriptionPlan?.DiscountPercentage is not null)
        {
            subscriptionDiscount = subtotal * (decimal)subscriptionPlan.DiscountPercentage.Value / 100;
            subtotal -= subscriptionDiscount;
        }

        return new ProductPriceResult
        {
            BasePrice = basePrice,
            OptionsTotal = optionsTotal,
            Quantity = quantity,
            TierDiscount = tierResult.TierDiscount,
            ProductDiscount = productDiscount,
            SubscriptionDiscount = subscriptionDiscount,
            Total = subtotal,
            UnitPrice = subtotal / quantity,
            PriceTier = tierResult.ToApplication(),
            OptionBreakdown = optionDetails
        };
    }

    private decimal ApplyDiscount(decimal amount, double discountRate)
    {
        return amount * (1 - (decimal)discountRate / 100);
    }

    private decimal CalculateOptionsTotal(
        Product product,
        IEnumerable<ProductOptionValue> selectedOptionValues,
        List<ProductOptionPriceDetail> breakdown)
    {
        decimal optionsTotal = 0;

        foreach (var selected in selectedOptionValues)
        {
            var option = product.ProductOptions
                .FirstOrDefault(po => po.Option.Id == selected.OptionId)?.Option;

            if (option is null)
            {
                throw new InvalidOperationException($"Option with ID '{selected.OptionId}' not found on product '{product.Name}'.");
            }

            decimal price = 0;

            switch (option)
            {
                case NumericalValueOption numericalValueOption:
                    price = numericalValueOption.Price.GetValueOrDefault() * (selected.NumericValue ?? 0);
                    break;

                case SelectableOption selectableOption:
                    price = selectableOption.Price.GetValueOrDefault();
                    break;

                case ChoiceOption choiceOption:
                    var chosenValue = choiceOption.Values.FirstOrDefault(v => v.Id == selected.ChoiceValueId);
                    if (chosenValue is not null)
                    {
                        price = chosenValue.Price.GetValueOrDefault();
                    }
                    break;
            }

            if (price > 0)
            {
                breakdown.Add(new ProductOptionPriceDetail
                {
                    OptionId = selected.OptionId,
                    Price = price
                });

                optionsTotal += price;
            }
        }

        return optionsTotal;
    }

    private static TierCalculationResult CalculateTierAdjustment(ProductPrice price, int quantity)
    {
        var baseSubtotal = price.Price * quantity;

        var tier = price.PriceTiers
            .Where(t => t.FromQuantity <= quantity && (t.ToQuantity is null || quantity <= t.ToQuantity))
            .OrderByDescending(t => t.FromQuantity)
            .ThenByDescending(t => t.ToQuantity ?? int.MaxValue)
            .FirstOrDefault();

        if (tier is null)
        {
            return TierCalculationResult.None(baseSubtotal, price.Price);
        }

        return tier.TierType switch
        {
            ProductPriceTierType.PricePerUnit => CalculatePricePerUnitTier(tier, baseSubtotal, quantity),
            ProductPriceTierType.DiscountPerUnit => CalculateDiscountPerUnitTier(price, tier, baseSubtotal, quantity),
            ProductPriceTierType.DiscountPerAdditionalUnit => CalculateDiscountPerAdditionalUnitTier(price, tier, baseSubtotal, quantity),
            _ => TierCalculationResult.None(baseSubtotal, price.Price)
        };
    }

    private static TierCalculationResult CalculatePricePerUnitTier(ProductPriceTier tier, decimal baseSubtotal, int quantity)
    {
        var subtotalAfterTier = tier.Value * quantity;
        var tierDiscount = Math.Max(0, baseSubtotal - subtotalAfterTier);

        return new TierCalculationResult(
            tier,
            ClampNonNegative(subtotalAfterTier),
            tierDiscount,
            quantity,
            tier.Value);
    }

    private static TierCalculationResult CalculateDiscountPerUnitTier(ProductPrice price, ProductPriceTier tier, decimal baseSubtotal, int quantity)
    {
        var discountPerUnit = Math.Min(tier.Value, price.Price);
        var tierDiscount = discountPerUnit * quantity;
        var subtotalAfterTier = ClampNonNegative(baseSubtotal - tierDiscount);

        return new TierCalculationResult(
            tier,
            subtotalAfterTier,
            tierDiscount,
            quantity,
            quantity > 0 ? subtotalAfterTier / quantity : price.Price);
    }

    private static TierCalculationResult CalculateDiscountPerAdditionalUnitTier(ProductPrice price, ProductPriceTier tier, decimal baseSubtotal, int quantity)
    {
        var upperBound = tier.ToQuantity ?? quantity;
        var eligibleUnits = Math.Min(quantity, upperBound) - tier.FromQuantity + 1;
        if (eligibleUnits < 0)
        {
            eligibleUnits = 0;
        }

        var discountPerUnit = Math.Min(tier.Value, price.Price);
        var tierDiscount = discountPerUnit * eligibleUnits;
        var subtotalAfterTier = ClampNonNegative(baseSubtotal - tierDiscount);

        return new TierCalculationResult(
            tier,
            subtotalAfterTier,
            tierDiscount,
            eligibleUnits,
            quantity > 0 ? subtotalAfterTier / quantity : price.Price);
    }

    private static decimal ClampNonNegative(decimal value) => value < 0 ? 0 : value;

    private sealed record TierCalculationResult(ProductPriceTier? Tier, decimal SubtotalAfterTier, decimal TierDiscount, int DiscountedUnits, decimal EffectiveUnitPrice)
    {
        public static TierCalculationResult None(decimal subtotal, decimal unitPrice) => new(null, subtotal, 0m, 0, unitPrice);

        public ProductPriceTierApplication? ToApplication()
        {
            if (Tier is null)
            {
                return null;
            }

            return new ProductPriceTierApplication
            {
                Id = Tier.Id,
                FromQuantity = Tier.FromQuantity,
                ToQuantity = Tier.ToQuantity,
                TierType = Tier.TierType,
                Value = Tier.Value,
                DiscountedUnits = DiscountedUnits,
                EffectiveUnitPrice = EffectiveUnitPrice
            };
        }
    }
}

public class ProductOptionValue
{
    public string OptionId { get; set; } = default!;
    public decimal? NumericValue { get; set; }
    public string? ChoiceValueId { get; set; }
}

public class ProductPriceResult
{
    public decimal BasePrice { get; set; }
    public decimal OptionsTotal { get; set; }
    public int Quantity { get; set; } = 1;
    public decimal TierDiscount { get; set; }
    public decimal ProductDiscount { get; set; }
    public decimal SubscriptionDiscount { get; set; }
    public decimal Total { get; set; }
    public decimal UnitPrice { get; set; }

    public decimal DiscountAmount => TierDiscount + ProductDiscount + SubscriptionDiscount;

    public ProductPriceTierApplication? PriceTier { get; set; }

    public List<ProductOptionPriceDetail>? OptionBreakdown { get; set; }
}

public class ProductOptionPriceDetail
{
    public string OptionId { get; set; } = default!;
    public decimal Price { get; set; }
}

public class ProductPriceTierApplication
{
    public string Id { get; set; } = default!;
    public int FromQuantity { get; set; }
    public int? ToQuantity { get; set; }
    public ProductPriceTierType TierType { get; set; }
    public decimal Value { get; set; }
    public int DiscountedUnits { get; set; }
    public decimal EffectiveUnitPrice { get; set; }
}
