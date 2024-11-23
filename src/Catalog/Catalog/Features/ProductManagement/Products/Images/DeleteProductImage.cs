using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Images;

public sealed record DeleteProductImage(string OrganizationId, string IdOrHandle, string ProductImageId) : IRequest<Result<ProductImageDto>>
{
    public sealed class Handler(IProductImageUploader productImageUploader, IPublishEndpoint publishEndpoint, CatalogContext catalogContext = default!) : IRequestHandler<DeleteProductImage, Result<ProductImageDto>>
    {
        public async Task<Result<ProductImageDto>> Handle(DeleteProductImage request, CancellationToken cancellationToken)
        {
            var isId = int.TryParse(request.IdOrHandle, out var id);

            var product = isId ?
                await catalogContext.Products
                        .InOrganization(request.OrganizationId)
                        .IncludeImages()
                        .FirstOrDefaultAsync(product => product.Id == id, cancellationToken)
                : await catalogContext.Products
                        .InOrganization(request.OrganizationId)
                        .IncludeImages()
                        .FirstOrDefaultAsync(product => product.Handle == request.IdOrHandle, cancellationToken);

            if (product is null)
            {
                return Result.Failure<ProductImageDto>(Errors.ProductNotFound);
            }

            var productImage = product.Images.FirstOrDefault(x => x.Id == request.ProductImageId);

            // Delete image

            product.RemoveImage(productImage);

            await catalogContext.SaveChangesAsync(cancellationToken);

            await publishEndpoint.Publish(new Catalog.Contracts.ProductImageUpdated
            {
                ProductId = product.Id,
                ImageUrl = product.Image.Url
            });

            return Result.SuccessWith(product.Image.ToDto());
        }
    }
}