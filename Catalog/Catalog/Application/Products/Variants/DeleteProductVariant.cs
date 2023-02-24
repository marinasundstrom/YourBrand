using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products.Variants;

public record DeleteProductVariant(string ProductId, string ProductVariantId) : IRequest
{
    public class Handler : IRequestHandler<DeleteProductVariant>
    {
        private readonly ICatalogContext _context;

        public Handler(ICatalogContext context)
        {
            _context = context;
        }

        public async Task Handle(DeleteProductVariant request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .AsSplitQuery()
                .Include(pv => pv.Variants)
                .FirstAsync(x => x.Id == request.ProductId);

            var variant = product.Variants.First(x => x.Id == request.ProductVariantId);

            product.Variants.Remove(variant);
            _context.ProductVariants.Remove(variant);

            await _context.SaveChangesAsync();

        }
    }
}
