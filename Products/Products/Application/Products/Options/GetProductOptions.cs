using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products.Options;

public record GetProductOptions(string ProductId) : IRequest<IEnumerable<ApiOption>>
{
    public class Handler : IRequestHandler<GetProductOptions, IEnumerable<ApiOption>>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApiOption>> Handle(GetProductOptions request, CancellationToken cancellationToken)
        {
            var options = await _context.Options
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Group)
                .Include(pv => pv.Values)
                .Include(o => o.DefaultValue)
                .Where(p => p.Products.Any(x => x.Id == request.ProductId))
                .ToArrayAsync();

            return options.Select(x => new ApiOption(x.Id, x.Name, x.Description, x.OptionType == Domain.Enums.OptionType.Single ? OptionType.Single : OptionType.Multiple, x.Group == null ? null : new ApiOptionGroup(x.Group.Id, x.Group.Name, x.Group.Description, x.Group.Seq, x.Group.Min, x.Group.Max), x.SKU, x.Price, x.IsSelected,
                x.Values.Select(x => new ApiOptionValue(x.Id, x.Name, x.SKU, x.Price, x.Seq)),
                x.DefaultValue == null ? null : new ApiOptionValue(x.DefaultValue.Id, x.DefaultValue.Name, x.DefaultValue.SKU, x.DefaultValue.Price, x.DefaultValue.Seq)));
        }
    }
}
