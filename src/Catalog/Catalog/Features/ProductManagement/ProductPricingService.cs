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

        var optionsTotal = CalculateOptionsTotal(product, selectedOptionValues);

        decimal subtotal = basePrice;

        if (product.PricingModel == PricingModel.OptionsBased)
        {
            subtotal += optionsTotal;
        }

        decimal discountAmount = 0;

        if (product.DiscountRate.HasValue)
        {
            if (product.DiscountApplicationMode == DiscountApplicationMode.OnBasePrice)
            {
                var discountedBase = basePrice * (1 - (decimal)product.DiscountRate.Value / 100);
                discountAmount = basePrice - discountedBase;
                subtotal = discountedBase + optionsTotal;
            }
            else if (product.DiscountApplicationMode == DiscountApplicationMode.AfterOptions)
            {
                discountAmount = subtotal * (decimal)product.DiscountRate.Value / 100;
                subtotal -= discountAmount;
            }
        }

        // Optional: override with subscription plan discount
        if (subscriptionPlan?.DiscountPercentage is not null)
        {
            discountAmount = subtotal * (decimal)subscriptionPlan.DiscountPercentage.Value / 100;
            subtotal -= discountAmount;
        }

        return new ProductPriceResult
        {
            BasePrice = basePrice,
            OptionsTotal = optionsTotal,
            DiscountAmount = discountAmount,
            Total = subtotal
        };
    }

    private decimal CalculateOptionsTotal(Product product, IEnumerable<ProductOptionValue> selectedOptionValues)
    {
        decimal optionsTotal = 0;

        foreach (var selected in selectedOptionValues)
        {
            var option = product.ProductOptions
                .FirstOrDefault(po => po.Option.Id == selected.OptionId)?
                .Option;

            if (option is null)
            {
                throw new Exception("Option not found");
            }

            if (option is NumericalValueOption numericalValueOption)
            {
                optionsTotal += numericalValueOption.Price.GetValueOrDefault() * (selected.NumericValue ?? 0);
            }
            else if (option is SelectableOption selectableOption)
            {
                optionsTotal += selectableOption.Price.GetValueOrDefault();
            }
            else if (option is ChoiceOption choiceOption)
            {
                var chosenValue = choiceOption.Values.FirstOrDefault(v => v.Id == selected.ChoiceValueId);
                if (chosenValue is not null)
                {
                    optionsTotal += chosenValue.Price.GetValueOrDefault();
                }
            }
        }

        return optionsTotal;
    }
}

public class ProductOptionValue
{
    public string OptionId { get; set; }
    public decimal? NumericValue { get; set; }
    public string? ChoiceValueId { get; set; }
}

public class ProductPriceResult
{
    public decimal BasePrice { get; set; }
    public decimal OptionsTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal Total { get; set; }
    
    //public List<ProductOptionPriceDetail>? OptionBreakdown { get; set; } // Om du vill visa options individuellt
}

public class ProductOptionPriceDetail
{
    public string OptionId { get; set; } = default!;
    public decimal Price { get; set; }
}