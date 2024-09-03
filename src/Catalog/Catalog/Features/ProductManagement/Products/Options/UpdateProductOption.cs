using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Features.ProductManagement.Options;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Options;

public record UpdateProductOption(string OrganizationId, long ProductId, string OptionId, UpdateProductOptionData Data) : IRequest<OptionDto>
{
    public class Handler(CatalogContext context) : IRequestHandler<UpdateProductOption, OptionDto>
    {
        public async Task<OptionDto> Handle(UpdateProductOption request, CancellationToken cancellationToken)
        {
            var product = await context.Products
                .InOrganization(request.OrganizationId)
                .AsNoTracking()
                .FirstAsync(x => x.Id == request.ProductId);

            var option = await context.Options
                .InOrganization(request.OrganizationId)
                .Include(x => (x as ChoiceOption)!.Values)
                .Include(x => x.Group)
                .FirstAsync(x => x.Id == request.OptionId);

            var group = await context.OptionGroups
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Data.GroupId);

            option.Name = request.Data.Name;
            option.Description = request.Data.Description;
            option.Group = group;

            if (option.OptionType == Domain.Enums.OptionType.YesOrNo)
            {
                if (option is SelectableOption selectableOption)
                {
                    selectableOption.IsSelected = request.Data.IsSelected.GetValueOrDefault();
                    selectableOption.SKU = request.Data.SKU;
                    selectableOption.Price = request.Data.Price;
                }
            }
            else if (option.OptionType == Domain.Enums.OptionType.NumericalValue)
            {
                if (option is NumericalValueOption numericalValue)
                {
                    numericalValue.MinNumericalValue = request.Data.MinNumericalValue;
                    numericalValue.MaxNumericalValue = request.Data.MaxNumericalValue;
                    numericalValue.DefaultNumericalValue = request.Data.DefaultNumericalValue;
                    numericalValue.Price = request.Data.Price;
                }
            }
            else if (option.OptionType == Domain.Enums.OptionType.TextValue)
            {
                if (option is TextValueOption textValueOption)
                {
                    textValueOption.TextValueMinLength = request.Data.TextValueMinLength;
                    textValueOption.TextValueMaxLength = request.Data.TextValueMaxLength;
                    textValueOption.DefaultTextValue = request.Data.DefaultTextValue;
                }
            }
            else if (option.OptionType == Domain.Enums.OptionType.Choice)
            {
                if (option is ChoiceOption choiceOption)
                {
                    foreach (var v in request.Data.Values)
                    {
                        if (v.Id == null)
                        {
                            var value = new OptionValue(v.Name)
                            {
                                SKU = v.SKU,
                                Price = v.Price
                            };

                            choiceOption.AddValue(value);
                            context.OptionValues.Add(value);
                        }
                        else
                        {
                            var value = choiceOption!.Values.First(x => x.Id == v.Id);

                            value.Name = v.Name;
                            value.SKU = v.SKU;
                            value.Price = v.Price;
                        }
                    }

                    choiceOption!.DefaultValueId = choiceOption!.Values.FirstOrDefault(x => x.Id == request.Data.DefaultOptionValueId)?.Id;

                    foreach (var v in choiceOption!.Values.ToList())
                    {
                        if (context.Entry(v).State == EntityState.Added)
                            continue;

                        var value = request.Data.Values.FirstOrDefault(x => x.Id == v.Id);

                        if (value is null)
                        {
                            choiceOption!.RemoveValue(v);
                        }
                    }
                }
            }

            await context.SaveChangesAsync();

            return option.ToDto();
        }
    }
}