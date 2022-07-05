using Azure.Storage.Blobs;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Products.Domain;

namespace YourBrand.Products.Application.Products.Variants;

public record UploadProductVariantImage(string ProductId, string VariantId, string FileName, Stream Stream) : IRequest<string?>
{
    public class Handler : IRequestHandler<UploadProductVariantImage, string?>
    {
        private readonly IProductsContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public Handler(IProductsContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string?> Handle(UploadProductVariantImage request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
            .Include(x => x.Variants)
            .FirstAsync(x => x.Id == request.ProductId);

            var variant = await _context.ProductVariants
                .FirstAsync(x => x.Id == request.VariantId);

            var blobContainerClient = _blobServiceClient.GetBlobContainerClient("images");

#if DEBUG
            await blobContainerClient.CreateIfNotExistsAsync();
#endif

            var response = await blobContainerClient.UploadBlobAsync(request.FileName, request.Stream);

            if (variant.Image is not null)
            {
                await blobContainerClient.DeleteBlobAsync(variant.Image);
            }

            variant.Image = request.FileName;

            await _context.SaveChangesAsync();

            return GetImageUrl(variant.Image);
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}