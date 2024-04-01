using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products;

public sealed record UpdateProductListingState(string IdOrHandle, ProductListingState ListingState) : IRequest<Result>
{
    public sealed class Handler(IPublishEndpoint publishEndpoint, CatalogContext catalogContext = default!) : IRequestHandler<UpdateProductListingState, Result>
    {
        public async Task<Result> Handle(UpdateProductListingState request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrHandle, out var id);

            var product = isId ?
                await catalogContext.Products.FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                : await catalogContext.Products.FirstOrDefaultAsync(product => product.Handle == request.IdOrHandle, cancellationToken);

            if (product is null)
            {
                return Result.Failure(Errors.ProductNotFound);
            }

            product.ListingState = (Domain.Enums.ProductListingState)request.ListingState;

            await catalogContext.SaveChangesAsync(cancellationToken);

            await publishEndpoint.Publish(new Catalog.Contracts.ProductListingStateUpdated
            {
                ProductId = product.Id,
                ListingState = (Contracts.ProductListingState)product.ListingState
            });

            return Result.Success();
        }
    }
}