using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Variants;

public record GetProductVariantAttributes(long ProductId, long ProductVariantId) : IRequest<IEnumerable<ProductVariantAttributeDto>>
{
    public class Handler(CatalogContext context) : IRequestHandler<GetProductVariantAttributes, IEnumerable<ProductVariantAttributeDto>>
    {
        public async Task<IEnumerable<ProductVariantAttributeDto>> Handle(GetProductVariantAttributes request, CancellationToken cancellationToken)
        {
            var variantOptionValues = await context.ProductAttributes
                .AsSplitQuery()
                .AsNoTracking()
                .Include(pv => pv.Value)
                .Include(pv => pv.Attribute)
                //.ThenInclude(o => o.DefaultValue)
                .Include(pv => pv.Product)
                .ThenInclude(p => p.Parent)
                .Where(pv => pv.Product.Parent!.Id == request.ProductId && pv.Product.Id == request.ProductVariantId)
                .ToArrayAsync();

            return variantOptionValues.Select(x => x.ToDto());
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}