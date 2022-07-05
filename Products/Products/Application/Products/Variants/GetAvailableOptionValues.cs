using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;
using YourBrand.Products.Domain.Entities;

namespace YourBrand.Products.Application.Products.Variants;

public record GetAvailableOptionValues(string ProductId, string OptionId, IDictionary<string, string?> SelectedOptions) : IRequest<IEnumerable<ApiOptionValue>>
{
    public class Handler : IRequestHandler<GetAvailableOptionValues, IEnumerable<ApiOptionValue>>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApiOptionValue>> Handle(GetAvailableOptionValues request, CancellationToken cancellationToken)
        {
            IEnumerable<ProductVariant> variants = await _context.ProductVariants
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Product)
                .Include(pv => pv.Values)
                .ThenInclude(pv => pv.Attribute)
                .Include(pv => pv.Values)
                .ThenInclude(pv => pv.Value)
                .Where(pv => pv.Product.Id == request.ProductId)
                .ToArrayAsync();

            foreach (var selectedOption in request.SelectedOptions)
            {
                if (selectedOption.Value is null)
                    continue;

                variants = variants.Where(x => x.Values.Any(vv => vv.Attribute.Id == selectedOption.Key && vv.Value.Id == selectedOption.Value));
            }

            var values = variants
                .SelectMany(x => x.Values)
                .DistinctBy(x => x.Attribute)
                .Where(x => x.Attribute.Id == request.OptionId)
                .Select(x => x.Value);

            return values.Select(x => new ApiOptionValue(x.Id, x.Name, x.Name, x.Price, x.Seq));
        }
    }
}
