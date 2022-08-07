using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products.Variants;

public record GetProductVariantAttributes(string ProductId, string ProductVariantId) : IRequest<IEnumerable<ProductVariantAttributeDto>>
{
    public class Handler : IRequestHandler<GetProductVariantAttributes, IEnumerable<ProductVariantAttributeDto>>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProductVariantAttributeDto>> Handle(GetProductVariantAttributes request, CancellationToken cancellationToken)
        {
            var variantOptionValues = await _context.ProductVariantAttributeValues
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Value)
                .Include(pv => pv.Attribute)
                //.ThenInclude(o => o.DefaultValue)
                .Include(pv => pv.Variant)
                .ThenInclude(p => p.Product)
                .Where(pv => pv.Variant.Product.Id == request.ProductId && pv.Variant.Id == request.ProductVariantId)
                .ToArrayAsync();

            return variantOptionValues.Select(x => x.ToDto());
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}