using YourBrand.Catalog.Persistence;

using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Images;

public sealed record UpdateProductImage(string IdOrHandle, string ProductImageId, string? Title, string? Text) : IRequest<Result<ProductImageDto>>
{
    public sealed class Handler(IProductImageUploader productImageUploader, IPublishEndpoint publishEndpoint, CatalogContext catalogContext = default!) : IRequestHandler<UpdateProductImage, Result<ProductImageDto>>
    {
        public async Task<Result<ProductImageDto>> Handle(UpdateProductImage request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrHandle, out var id);

            var product = isId ?
                await catalogContext.Products.IncludeImages().FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                : await catalogContext.Products.IncludeImages().FirstOrDefaultAsync(product => product.Handle == request.IdOrHandle, cancellationToken);

            if (product is null)
            {
                return Result.Failure<ProductImageDto>(Errors.ProductNotFound);
            }

            var productImage = product.Images.First(x => x.Id == request.ProductImageId);

            productImage.Title = request.Title;
            productImage.Text = request.Text;

            await catalogContext.SaveChangesAsync(cancellationToken);

            /*
            await publishEndpoint.Publish(new Catalog.Contracts.ProductImageUpdated
            {
                ProductId = product.Id,
                ImageUrl = product.Image
            });
            */

            return Result.Success(productImage.ToDto());
        }
    }
}