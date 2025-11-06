using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Features.ProductManagement.Products;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.PriceTiers;

public sealed record DeleteProductPriceTier(string OrganizationId, string IdOrHandle, string TierId) : IRequest<Result>
{
    public sealed class Handler(CatalogContext catalogContext) : IRequestHandler<DeleteProductPriceTier, Result>
    {
        public async Task<Result> Handle(DeleteProductPriceTier request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrHandle, out var id);

            var productQuery = catalogContext.Products
                .InOrganization(request.OrganizationId)
                .Include(x => x.Prices)
                    .ThenInclude(x => x.PriceTiers);

            var product = isId
                ? await productQuery.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                : await productQuery.FirstOrDefaultAsync(x => x.Handle == request.IdOrHandle, cancellationToken);

            if (product is null)
            {
                return Result.Failure(Errors.ProductNotFound);
            }

            var price = product.Prices.FirstOrDefault();

            if (price is null)
            {
                return Result.Failure(Errors.ProductPriceMissing);
            }

            var tier = price.GetPriceTier(request.TierId) ?? await catalogContext.ProductPriceTiers
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.TierId, cancellationToken);

            if (tier is null)
            {
                return Result.Failure(Errors.ProductPriceTierNotFound);
            }

            catalogContext.ProductPriceTiers.Remove(tier);

            await catalogContext.SaveChangesAsync(cancellationToken);

            return Result.Success;
        }
    }
}
