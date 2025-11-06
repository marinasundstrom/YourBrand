using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain.Entities;
using YourBrand.Catalog.Features.ProductManagement.Products;
using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.PriceTiers;

public sealed record CreateProductPriceTier(
    string OrganizationId,
    string IdOrHandle,
    int FromQuantity,
    int? ToQuantity,
    ProductPriceTierType TierType,
    decimal Value) : IRequest<Result<ProductPriceTierDto>>
{
    public sealed class Handler(CatalogContext catalogContext) : IRequestHandler<CreateProductPriceTier, Result<ProductPriceTierDto>>
    {
        public async Task<Result<ProductPriceTierDto>> Handle(CreateProductPriceTier request, CancellationToken cancellationToken)
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

            var priceTier = new ProductPriceTier(price.Id, request.FromQuantity, request.ToQuantity, request.TierType, request.Value)
            {
                OrganizationId = product.OrganizationId,
                TenantId = product.TenantId
            };

            price.AddPriceTier(priceTier);

            await catalogContext.SaveChangesAsync(cancellationToken);

            return Result.SuccessWith(priceTier.ToDto());
        }
    }
}
