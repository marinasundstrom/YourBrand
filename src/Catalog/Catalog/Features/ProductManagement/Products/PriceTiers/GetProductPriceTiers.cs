using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Features.ProductManagement.Products;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.PriceTiers;

public sealed record GetProductPriceTiers(string OrganizationId, string IdOrHandle) : IRequest<Result<IEnumerable<ProductPriceTierDto>>>
{
    public sealed class Handler(CatalogContext catalogContext) : IRequestHandler<GetProductPriceTiers, Result<IEnumerable<ProductPriceTierDto>>>
    {
        public async Task<Result<IEnumerable<ProductPriceTierDto>>> Handle(GetProductPriceTiers request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrHandle, out var id);

            var productQuery = catalogContext.Products
                .InOrganization(request.OrganizationId)
                .AsNoTracking()
                .Include(x => x.Prices)
                    .ThenInclude(x => x.PriceTiers);

            var product = isId
                ? await productQuery.FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                : await productQuery.FirstOrDefaultAsync(x => x.Handle == request.IdOrHandle, cancellationToken);

            if (product is null)
            {
                return Result.Failure<IEnumerable<ProductPriceTierDto>>(Errors.ProductNotFound);
            }

            var price = product.Prices.FirstOrDefault();

            if (price is null)
            {
                return Result.Failure<IEnumerable<ProductPriceTierDto>>(Errors.ProductPriceMissing);
            }

            var tiers = price.PriceTiers
                .OrderBy(x => x.FromQuantity)
                .ThenBy(x => x.ToQuantity ?? int.MaxValue)
                .Select(x => x.ToDto());

            return Result.SuccessWith(tiers);
        }
    }
}
