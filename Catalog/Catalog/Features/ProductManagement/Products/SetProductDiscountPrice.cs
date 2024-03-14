using YourBrand.Catalog.Persistence;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record SetProductDiscountPrice(string IdOrHandle, decimal DiscountPrice) : IRequest<Result>
{
    public sealed class Handler(IPublishEndpoint publishEndpoint, CatalogContext catalogContext = default!) : IRequestHandler<SetProductDiscountPrice, Result>
    {
        public async Task<Result> Handle(SetProductDiscountPrice request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrHandle, out var id);

            var product = isId ?
                await catalogContext.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                : await catalogContext.Products.FirstOrDefaultAsync(product => product.Handle == request.IdOrHandle, cancellationToken);

            if (product is null)
            {
                return Result.Failure(Errors.ProductNotFound);
            }

            if (product.RegularPrice is not null)
            {
                return Result.Failure(Errors.ProductAlreadyDiscounted);
            }

            product.SetDiscountPrice(request.DiscountPrice);

            await catalogContext.SaveChangesAsync(cancellationToken);

            await publishEndpoint.Publish(new Catalog.Contracts.ProductPriceUpdated
            {
                ProductId = product.Id,
                NewPrice = product.Price,
                RegularPrice = product.RegularPrice,
                DiscountRate = product.DiscountRate
            });

            return Result.Success();
        }
    }
}