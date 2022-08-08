using Azure.Storage.Blobs;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products.Variants;

public record UploadProductVariantImage(string ProductId, string VariantId, string FileName, Stream Stream) : IRequest<string?>
{
    public class Handler : IRequestHandler<UploadProductVariantImage, string?>
    {
        private readonly ICatalogContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public Handler(ICatalogContext context, BlobServiceClient blobServiceClient)
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

            var blobId = $"{variant.Id}:{request.FileName}";

            await blobContainerClient.DeleteBlobIfExistsAsync(blobId);

            var response = await blobContainerClient.UploadBlobAsync(blobId, request.Stream);

            variant.Image = blobId;

            await _context.SaveChangesAsync();

            return GetImageUrl(variant.Image);
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}