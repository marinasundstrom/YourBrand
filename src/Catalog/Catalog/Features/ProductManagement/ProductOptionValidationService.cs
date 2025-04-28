using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Features.ProductManagement;

public class ProductOptionValidationService
{
    public void ValidateSelectedOptions(Product product, IEnumerable<ProductOptionValue> selectedOptionValues)
    {
        foreach (var selected in selectedOptionValues)
        {
            var productOption = product.ProductOptions
                .FirstOrDefault(po => po.Option.Id == selected.OptionId);

            if (productOption is null)
            {
                throw new ValidationException($"Invalid option: {selected.OptionId} is not allowed for this product.");
            }

            var option = productOption.Option;

            switch (option)
            {
                case NumericalValueOption numericalValueOption:
                    if (!selected.NumericValue.HasValue)
                    {
                        throw new ValidationException($"Option {option.Id} requires a numeric value.");
                    }

                    if (numericalValueOption.MinNumericalValue.HasValue && selected.NumericValue < numericalValueOption.MinNumericalValue)
                    {
                        throw new ValidationException($"Option {option.Id} must be at least {numericalValueOption.MinNumericalValue}.");
                    }

                    if (numericalValueOption.MaxNumericalValue.HasValue && selected.NumericValue > numericalValueOption.MaxNumericalValue)
                    {
                        throw new ValidationException($"Option {option.Id} must be at most {numericalValueOption.MaxNumericalValue}.");
                    }
                    break;

                case SelectableOption selectableOption:
                    // You could check if selectableOption is required or only allowed in certain conditions
                    // Example: IsSelected could be enforced here if needed
                    break;

                case ChoiceOption choiceOption:
                    var chosenValue = choiceOption.Values.FirstOrDefault(v => v.Id == selected.ChoiceValueId);
                    if (chosenValue is null)
                    {
                        throw new ValidationException($"Invalid choice selection for option {option.Id}.");
                    }
                    break;

                default:
                    throw new ValidationException($"Unknown option type for option {option.Id}.");
            }
        }

        // Optionally: check for required options that are missing
        var requiredOptions = product.ProductOptions.Where(po => po.Option.IsRequired);
        foreach (var required in requiredOptions)
        {
            if (!selectedOptionValues.Any(v => v.OptionId == required.Option.Id))
            {
                throw new ValidationException($"Required option {required.Option.Id} is missing.");
            }
        }
    }
}

public class ValidationException : Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}