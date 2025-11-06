using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Features.ProductManagement.Products;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.PriceTiers;

public sealed record UpdateProductPriceTier(
    string OrganizationId,
    string IdOrHandle,
    string TierId,
    int FromQuantity,
    int? ToQuantity,
    ProductPriceTierType TierType,
    decimal Value) : IRequest<Result<ProductPriceTierDto>>
{
    public sealed class Handler(CatalogContext catalogContext) : IRequestHandler<UpdateProductPriceTier, Result<ProductPriceTierDto>>
    {
        public async Task<Result<ProductPriceTierDto>> Handle(UpdateProductPriceTier request, CancellationToken cancellationToken)
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
                return Result.Failure<ProductPriceTierDto>(Errors.ProductNotFound);
            }

            var price = product.Prices.FirstOrDefault();

            if (price is null)
            {
                return Result.Failure<ProductPriceTierDto>(Errors.ProductPriceMissing);
            }

            var tier = price.GetPriceTier(request.TierId) ?? await catalogContext.ProductPriceTiers
                .InOrganization(request.OrganizationId)
                .FirstOrDefaultAsync(x => x.Id == request.TierId, cancellationToken);

            if (tier is null)
            {
                return Result.Failure<ProductPriceTierDto>(Errors.ProductPriceTierNotFound);
            }

            price.UpdatePriceTier(tier, request.FromQuantity, request.ToQuantity, request.TierType, request.Value);

            await catalogContext.SaveChangesAsync(cancellationToken);

            return Result.SuccessWith(tier.ToDto());
        }
    }
}
