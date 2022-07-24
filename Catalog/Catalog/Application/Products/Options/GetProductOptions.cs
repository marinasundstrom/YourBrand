using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Application.Options;
using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products.Options;

public record GetProductOptions(string ProductId) : IRequest<IEnumerable<OptionDto>>
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
            var options = await _context.Options
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Group)
                .Include(pv => pv.Values)
                .Include(o => o.DefaultValue)
                .Where(p => p.Products.Any(x => x.Id == request.ProductId))
                .ToArrayAsync();

            return options.Select(x => new OptionDto(x.Id, x.Name, x.Description, x.OptionType == Domain.Enums.OptionType.YesOrNo ? OptionType.YesOrNo : OptionType.Choice, x.Group == null ? null : new OptionGroupDto(x.Group.Id, x.Group.Name, x.Group.Description, x.Group.Seq, x.Group.Min, x.Group.Max), x.SKU, x.Price, x.IsSelected,
                x.Values.Select(x => new OptionValueDto(x.Id, x.Name, x.SKU, x.Price, x.Seq)),
                x.DefaultValue == null ? null : new OptionValueDto(x.DefaultValue.Id, x.DefaultValue.Name, x.DefaultValue.SKU, x.DefaultValue.Price, x.DefaultValue.Seq)));
        }
    }
}
