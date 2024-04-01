using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record RestoreProductRegularPrice(string IdOrHandle) : IRequest<Result>
{
    public sealed class Handler(IPublishEndpoint publishEndpoint, CatalogContext catalogContext = default!) : IRequestHandler<RestoreProductRegularPrice, Result>
    {
        public async Task<Result> Handle(RestoreProductRegularPrice request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrHandle, out var id);

            var product = isId ?
                await catalogContext.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                : await catalogContext.Products.FirstOrDefaultAsync(product => product.Handle == request.IdOrHandle, cancellationToken);

            if (product is null)
            {
                return Result.Failure(Errors.ProductNotFound);
            }

            if (product.RegularPrice is null)
            {
                return Result.Failure(Errors.ProductNotDiscounted);
            }

            product.RestoreRegularPrice();

            await catalogContext.SaveChangesAsync(cancellationToken);

            await publishEndpoint.Publish(new Catalog.Contracts.ProductPriceUpdated
            {
                ProductId = product.Id,
                NewPrice = product.Price
            });

            return Result.Success();
        }
    }
}