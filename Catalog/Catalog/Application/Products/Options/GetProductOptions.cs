using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Application.Options;
using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products.Options;

public record GetProductOptions(string ProductId, string? VariantId) : IRequest<IEnumerable<OptionDto>>
{
    public class Handler : IRequestHandler<GetProductOptions, IEnumerable<OptionDto>>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OptionDto>> Handle(GetProductOptions request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.ProductOptions)
                .ThenInclude(pv => pv.Option)
                .ThenInclude(pv => pv.Group)
                .Include(pv => pv.ProductOptions)
                .ThenInclude(pv => pv.Option)
                .ThenInclude(pv => pv.Values)
                .Include(pv => pv.ProductOptions)
                .ThenInclude(pv => pv.Option)
                .ThenInclude(o => o.DefaultValue)
                .FirstAsync(p => p.Id == request.ProductId);

            var options = product.ProductOptions
                .Select(x => x.Option)
                .ToList();

            if(request.VariantId is not null) 
            {
                product = await _context.Products
                    .AsSplitQuery()
                    .AsNoTracking()
                    .Include(pv => pv.ProductVariantOptions)
                    .ThenInclude(pv => pv.Option)
                    .ThenInclude(pv => pv.Group)
                    .Include(pv => pv.ProductVariantOptions)
                    .ThenInclude(pv => pv.Option)
                    .ThenInclude(pv => pv.Values)
                    .Include(pv => pv.ProductVariantOptions)
                    .ThenInclude(pv => pv.Option)
                    .ThenInclude(o => o.DefaultValue)
                    .FirstAsync(p => p.Id == request.ProductId);

                options.AddRange(product.ProductVariantOptions.Select(x => x.Option));
            }

            return options.Select(x => new OptionDto(x.Id, x.Name, x.Description, (Application.OptionType)x.OptionType, x.Group == null ? null : new OptionGroupDto(x.Group.Id, x.Group.Name, x.Group.Description, x.Group.Seq, x.Group.Min, x.Group.Max), x.SKU, x.Price, x.IsSelected,
                x.Values.Select(x => new OptionValueDto(x.Id, x.Name, x.SKU, x.Price, x.Seq)),
                x.DefaultValue == null ? null : new OptionValueDto(x.DefaultValue.Id, x.DefaultValue.Name, x.DefaultValue.SKU, x.DefaultValue.Price, x.DefaultValue.Seq), x.MinNumericalValue, x.MaxNumericalValue, x.DefaultNumericalValue, x.TextValueMinLength, x.TextValueMaxLength, x.DefaultTextValue));
        }
    }
}
