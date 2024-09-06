using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Variants;

public record DeleteProductVariant(string OrganizationId, int ProductId, int ProductVariantId) : IRequest
{
    public class Handler(CatalogContext context) : IRequestHandler<DeleteProductVariant>
    {
        public async Task Handle(DeleteProductVariant request, CancellationToken cancellationToken)
        {
            var product = await context.Products
                .InOrganization(request.OrganizationId)
                .AsSplitQuery()
                .Include(pv => pv.Variants)
                .FirstAsync(x => x.Id == request.ProductVariantId);

            var variant = product.Variants.First(x => x.Id == request.ProductVariantId);

            product.RemoveVariant(variant);
            context.Products.Remove(variant);

            await context.SaveChangesAsync();

        }
    }
}