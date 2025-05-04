using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Features.ProductManagement;

public class ProductPricingService
{
    public ProductPriceResult CalculatePrice(
        Product product,
        IEnumerable<ProductOptionValue> selectedOptionValues,
        ProductSubscriptionPlan? subscriptionPlan = null)
    {
        var basePrice = product.Price;
        var optionDetails = new List<ProductOptionPriceDetail>();

        var optionsTotal = CalculateOptionsTotal(product, selectedOptionValues, optionDetails);

        decimal subtotal = basePrice;

        if (product.PricingModel == PricingModel.OptionsBased)
        {
            subtotal += optionsTotal;
        }

        decimal productDiscount = 0;

        if (product.DiscountRate.HasValue)
        {
            if (product.DiscountApplicationMode == DiscountApplicationMode.OnBasePrice)
            {
                var discountedBase = ApplyDiscount(basePrice, product.DiscountRate.Value);
                productDiscount = basePrice - discountedBase;
                subtotal = discountedBase + optionsTotal;
            }
            else if (product.DiscountApplicationMode == DiscountApplicationMode.AfterOptions)
            {
                productDiscount = subtotal * (decimal)product.DiscountRate.Value / 100;
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
            ProductDiscount = productDiscount,
            SubscriptionDiscount = subscriptionDiscount,
            Total = subtotal,
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
    public decimal ProductDiscount { get; set; }
    public decimal SubscriptionDiscount { get; set; }
    public decimal Total { get; set; }

    public List<ProductOptionPriceDetail>? OptionBreakdown { get; set; }
}

public class ProductOptionPriceDetail
{
    public string OptionId { get; set; } = default!;
    public decimal Price { get; set; }
}