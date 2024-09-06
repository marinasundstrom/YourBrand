using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Features.ProductManagement.Options;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Options;

public record CreateProductOption(string OrganizationId, int ProductId, CreateProductOptionData Data) : IRequest<OptionDto>
{
    public class Handler(CatalogContext context) : IRequestHandler<CreateProductOption, OptionDto>
    {
        public async Task<OptionDto> Handle(CreateProductOption request, CancellationToken cancellationToken)
        {
            var product = await context.Products
                .InOrganization(request.OrganizationId)
                .FirstAsync(x => x.Id == request.ProductId);

            var group = await context.OptionGroups
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.Data.GroupId);

            Option option = default!;

            if (request.Data.OptionType == OptionType.YesOrNo)
            {
                var selectableOption = new SelectableOption(request.Data.Name);

                selectableOption.IsSelected = request.Data.IsSelected.GetValueOrDefault();
                selectableOption.SKU = request.Data.SKU;
                selectableOption.Price = request.Data.Price;

                option = selectableOption;
            }
            else if (request.Data.OptionType == OptionType.NumericalValue)
            {
                var numericalValue = new NumericalValueOption(request.Data.Name);

                numericalValue.MinNumericalValue = request.Data.MinNumericalValue;
                numericalValue.MaxNumericalValue = request.Data.MaxNumericalValue;
                numericalValue.DefaultNumericalValue = request.Data.DefaultNumericalValue;

                option = numericalValue;
            }
            else if (request.Data.OptionType == OptionType.TextValue)
            {
                var textValueOption = new TextValueOption(request.Data.Name);

                textValueOption.TextValueMinLength = request.Data.TextValueMinLength;
                textValueOption.TextValueMaxLength = request.Data.TextValueMaxLength;
                textValueOption.DefaultTextValue = request.Data.DefaultTextValue;

                option = textValueOption;
            }
            else if (request.Data.OptionType == OptionType.Choice)
            {
                var choiceOption = new ChoiceOption(request.Data.Name);

                foreach (var v in request.Data.Values)
                {
                    var value = new OptionValue(v.Name)
                    {
                        SKU = v.SKU,
                        Price = v.Price
                    };

                    choiceOption!.AddValue(value);
                }

                choiceOption!.DefaultValueId = choiceOption!.Values.FirstOrDefault(x => x.Id == request.Data.DefaultOptionValueId)?.Id;

                option = choiceOption;
            }

            option.OrganizationId = request.OrganizationId;
            option.Description = request.Data.Description;
            option.Group = group;

            product.AddOption(option);

            if (product.HasVariants)
            {
                var variants = await context.Products
                    .InOrganization(request.OrganizationId)
                    .Where(x => x.ParentId == product.Id)
                    .Include(x => x.Options)
                    .ToArrayAsync(cancellationToken);

                foreach (var variant in product.Variants)
                {
                    variant.AddOption(option, true);
                }
            }

            await context.SaveChangesAsync();

            return option.ToDto();
        }
    }
}