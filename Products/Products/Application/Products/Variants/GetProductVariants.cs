using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products.Variants;

public record GetProductVariants(string ProductId) : IRequest<IEnumerable<ApiProductVariant>>
{
    public class Handler : IRequestHandler<GetProductVariants, IEnumerable<ApiProductVariant>>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ApiProductVariant>> Handle(GetProductVariants request, CancellationToken cancellationToken)
        {
            var variants = await _context.ProductVariants
            .AsSplitQuery()
            .AsNoTracking()
            .Include(pv => pv.Product)
            .Include(pv => pv.Values)
            .ThenInclude(pv => pv.Attribute)
            .Include(pv => pv.Values)
            .ThenInclude(pv => pv.Value)
            .Where(pv => pv.Product.Id == request.ProductId)
            .ToArrayAsync();

            return variants.Select(x => new ApiProductVariant(x.Id, x.Name, x.Description, x.SKU, GetImageUrl(x.Image), x.Price,
                x.Values.Select(x => new ApiProductVariantOption(x.Attribute.Id, x.Attribute.Name, x.Value.Name))));
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}
