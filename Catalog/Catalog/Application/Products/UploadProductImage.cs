using Azure.Storage.Blobs;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Domain;

namespace YourBrand.Catalog.Application.Products;

public record UploadProductImage(string ProductId, string FileName, Stream Stream) : IRequest<string?>
{
    public class Handler : IRequestHandler<UploadProductImage, string?>
    {
        private readonly ICatalogContext _context;
        private readonly BlobServiceClient _blobServiceClient;

        public Handler(ICatalogContext context, BlobServiceClient blobServiceClient)
        {
            _context = context;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<string?> Handle(UploadProductImage request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                               .FirstAsync(x => x.Id == request.ProductId);

            var blobContainerClient = _blobServiceClient.GetBlobContainerClient("images");

#if DEBUG
            await blobContainerClient.CreateIfNotExistsAsync();
#endif

            var blobId = $"{product.Id}:{request.FileName}";

            await blobContainerClient.DeleteBlobIfExistsAsync(blobId);

            var response = await blobContainerClient.UploadBlobAsync(blobId, request.Stream);

            product.Image = blobId;

            await _context.SaveChangesAsync();

            return GetImageUrl(product.Image);
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}