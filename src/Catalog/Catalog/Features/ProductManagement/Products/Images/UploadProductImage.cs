using MassTransit;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Images;

public sealed record UploadProductImage(string OrganizationId, string IdOrHandle, Stream Stream, string FileName, string ContentType, bool SetMainImage) : IRequest<Result<ProductImageDto>>
{
    public sealed class Handler(IProductImageUploader productImageUploader, IPublishEndpoint publishEndpoint, CatalogContext catalogContext = default!) : IRequestHandler<UploadProductImage, Result<ProductImageDto>>
    {
        public async Task<Result<ProductImageDto>> Handle(UploadProductImage request, CancellationToken cancellationToken)
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

            var imageUrl = await productImageUploader.UploadProductImage(product.Id, request.FileName, request.Stream, request.ContentType);

            var image = new Domain.Entities.ProductImage(request.FileName, string.Empty, imageUrl);
            image.OrganizationId = request.OrganizationId;

            product.AddImage(image);

            if (request.SetMainImage)
            {
                product.Image = image;
            }

            await catalogContext.SaveChangesAsync(cancellationToken);

            if (request.SetMainImage)
            {
                await publishEndpoint.Publish(new Catalog.Contracts.ProductImageUpdated
                {
                    ProductId = product.Id,
                    ImageUrl = product.Image.Url
                });
            }

            return Result.SuccessWith(image.ToDto());
        }
    }
}