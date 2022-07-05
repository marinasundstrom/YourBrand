using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products.Variants;

public record GetProductVariantOptions(string ProductId, string ProductVariantId) : IRequest<IEnumerable<ProductVariantDtoOption>>
{
    public class Handler : IRequestHandler<GetProductVariantOptions, IEnumerable<ProductVariantDtoOption>>
    {
        private readonly IProductsContext _context;

        public Handler(IProductsContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductVariantDtoOption>> Handle(GetProductVariantOptions request, CancellationToken cancellationToken)
        {
            var variantOptionValues = await _context.VariantValues
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Value)
                .Include(pv => pv.Attribute)
                //.ThenInclude(o => o.DefaultValue)
                .Include(pv => pv.Variant)
                .ThenInclude(p => p.Product)
                .Where(pv => pv.Variant.Product.Id == request.ProductId && pv.Variant.Id == request.ProductVariantId)
                .ToArrayAsync();

            return variantOptionValues.Select(x => new ProductVariantDtoOption(x.Attribute.Id, x.Attribute.Name, x.Value.Name));
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}
