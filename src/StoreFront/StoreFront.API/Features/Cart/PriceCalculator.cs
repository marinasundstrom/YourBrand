using System.Text.Json;

using YourBrand.Catalog;

namespace YourBrand.StoreFront.API.Features.Cart;

public class PriceCalculator
{
    public (decimal price, decimal? regularPrice) CalculatePrice(Product product, string data)
    {
        var options = JsonSerializer.Deserialize<IEnumerable<Option>>(data, new JsonSerializerOptions
        {
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        })!;

        var price = product!.Price;
        var regularPrice = product.RegularPrice;

        List<string> optionTexts = new List<string>();

        foreach (var option in options)
        {
            var productOption = product.Options.FirstOrDefault(x => x.Option.Id == option.Id);

            if (productOption is not null)
            {
                if (option.OptionType == 0)
                {
                    var isSelected = option.IsSelected.GetValueOrDefault();

                    if (!isSelected && isSelected != productOption.Option.IsSelected)
                    {
                        optionTexts.Add($"No {option.Name}");

                        continue;
                    }

                    if (isSelected)
                    {
                        price += option.Price.GetValueOrDefault();
                        regularPrice += option.Price.GetValueOrDefault();

                        if (option.Price is not null)
                        {
                            optionTexts.Add($"{option.Name} (+{option.Price?.ToString("c")})");
                        }
                        else
                        {
                            optionTexts.Add(option.Name);
                        }
                    }
                }
                else if (option.SelectedValueId is not null)
                {
                    var value = productOption.Option.Values.FirstOrDefault(x => x.Id == option.SelectedValueId)!;

                    price += value.Price.GetValueOrDefault();
                    regularPrice += value.Price.GetValueOrDefault();

                    if (value.Price is not null)
                    {
                        optionTexts.Add($"{value.Name} (+{value.Price?.ToString("c")})");
                    }
                    else
                    {
                        optionTexts.Add(value.Name);
                    }
                }
                else if (option.NumericalValue is not null)
                {
                    price += option.Price.GetValueOrDefault() * option.NumericalValue.GetValueOrDefault();
                    regularPrice += option.Price.GetValueOrDefault() * option.NumericalValue.GetValueOrDefault();

                    optionTexts.Add($"{option.NumericalValue} {option.Name}");
                }
            }
        }

        return (price, regularPrice);
    }
}