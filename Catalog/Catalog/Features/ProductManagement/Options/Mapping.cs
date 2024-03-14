using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Features.ProductManagement.Options;

public static class Mapping
{
    public static OptionDto ToDto(this Domain.Entities.Option option)
    {
        return new OptionDto(option.Id, option.Name, option.Description, (OptionType)option.OptionType, option.Group == null ? null : new OptionGroupDto(option.Group.Id, option.Group.Name, option.Group.Description, option.Group.Seq, option.Group.Min, option.Group.Max), option.IsRequired, (option as SelectableOption)?.SKU, (option as SelectableOption)?.Price ?? (option as NumericalValueOption)?.Price, (option as SelectableOption)?.IsSelected,
                (option as ChoiceOption)?.Values?.Select(x => new OptionValueDto(x.Id, x.Name, x.SKU, x.Price, x.Seq)) ?? Enumerable.Empty<OptionValueDto>(),
                (option as ChoiceOption)?.DefaultValue == null ? null : new OptionValueDto((option as ChoiceOption)?.DefaultValue?.Id!, (option as ChoiceOption)?.DefaultValue?.Name!, (option as ChoiceOption)?.DefaultValue?.SKU, (option as ChoiceOption)?.DefaultValue?.Price, (option as ChoiceOption)?.DefaultValue?.Seq), (option as NumericalValueOption)?.MinNumericalValue, (option as NumericalValueOption)?.MaxNumericalValue, (option as NumericalValueOption)?.DefaultNumericalValue, (option as TextValueOption)?.TextValueMinLength, (option as TextValueOption)?.TextValueMaxLength, (option as TextValueOption)?.DefaultTextValue);
    }

    public static OptionValueDto ToDto(this Domain.Entities.OptionValue option)
    {
        return new OptionValueDto(option.Id, option.Name, option.SKU, option.Price, option.Seq);
    }
}