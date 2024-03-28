using YourBrand.Catalog.Persistence;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record UpdateProductSku(string IdOrHandle, string Sku) : IRequest<Result>
{
    public sealed class Handler(IPublishEndpoint publishEndpoint, CatalogContext catalogContext = default!) : IRequestHandler<UpdateProductSku, Result>
    {
        public async Task<Result> Handle(UpdateProductSku request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrHandle, out var id);

            var product = isId ?
                await catalogContext.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                : await catalogContext.Products.FirstOrDefaultAsync(product => product.Handle == request.IdOrHandle, cancellationToken);

            if (product is null)
            {
                return Result.Failure(Errors.ProductNotFound);
            }

            var p = product;

            var skuInUse = await catalogContext.Products.AnyAsync(product => product.Id != p.Id && product.Sku == request.Sku, cancellationToken);

            if (skuInUse)
            {
                return Result.Failure(Errors.SkuAlreadyTaken);
            }

            product.Sku = request.Sku;

            await catalogContext.SaveChangesAsync(cancellationToken);

            await publishEndpoint.Publish(new Catalog.Contracts.ProductSkuUpdated
            {
                ProductId = product.Id,
                Sku = product.Sku
            });

            return Result.Success();
        }
    }
}