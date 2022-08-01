using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Application.Options;
using YourBrand.Catalog.Domain;
using YourBrand.Catalog.Domain.Entities;

namespace YourBrand.Catalog.Application.Products.Options;

public record CreateProductOption(string ProductId, ApiCreateProductOption Data) : IRequest<OptionDto>
{
    public class Handler : IRequestHandler<CreateProductOption, OptionDto>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<OptionDto> Handle(CreateProductOption request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
            .FirstAsync(x => x.Id == request.ProductId);

            var group = await _context.OptionGroups
                .FirstOrDefaultAsync(x => x.Id == request.Data.GroupId);

            Option option = new()
            {
                Id = Guid.NewGuid().ToString(),
                Name = request.Data.Name,
                Description = request.Data.Description,
                SKU = request.Data.SKU,
                Group = group,
                Price = request.Data.Price,
                OptionType = (Domain.Enums.OptionType)request.Data.OptionType
            };

            foreach (var v in request.Data.Values)
            {
                var value = new OptionValue
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = v.Name,
                    SKU = v.SKU,
                    Price = v.Price
                };

                option.Values.Add(value);
            }

            product.Options.Add(option);

            await _context.SaveChangesAsync();

            return new OptionDto(option.Id, option.Name, option.Description, (Application.OptionType)option.OptionType, option.Group == null ? null : new OptionGroupDto(option.Group.Id, option.Group.Name, option.Group.Description, option.Group.Seq, option.Group.Min, option.Group.Max), option.SKU, option.Price, option.IsSelected,
                option.Values.Select(x => new OptionValueDto(x.Id, x.Name, x.SKU, x.Price, x.Seq)),
                option.DefaultValue == null ? null : new OptionValueDto(option.DefaultValue.Id, option.DefaultValue.Name, option.DefaultValue.SKU, option.DefaultValue.Price, option.DefaultValue.Seq), option.MinNumericalValue, option.MaxNumericalValue, option.DefaultNumericalValue, option.TextValueMinLength, option.TextValueMaxLength, option.DefaultTextValue);
        
        }
    }
}
