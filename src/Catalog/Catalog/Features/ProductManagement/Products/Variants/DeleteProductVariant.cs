using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Variants;

public record DeleteProductVariant(long ProductId, long ProductVariantId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProductVariant>
    {
        private readonly CatalogContext _context;

        public Handler(CatalogContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteProductVariant request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .AsSplitQuery()
                .Include(pv => pv.Variants)
                .FirstAsync(x => x.Id == request.ProductVariantId);

            var variant = product.Variants.First(x => x.Id == request.ProductVariantId);

            product.RemoveVariant(variant);
            _context.Products.Remove(variant);

            await _context.SaveChangesAsync();

        }
    }
}