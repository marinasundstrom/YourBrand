using MediatR;

using YourBrand.Catalog.Persistence;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Variants;

public record UploadProductVariantImage(long ProductId, long VariantId, string FileName, Stream Stream) : IRequest<string?>
{
    public class Handler : IRequestHandler<UploadProductVariantImage, string?>
    {
        private readonly CatalogContext _context;
        //private readonly IBlobStorageService _blobStorageService;

        public Handler(CatalogContext context) //, IBlobStorageService blobStorageService)
        {
            _context = context;
            //_blobStorageService = blobStorageService;
        }

        public async Task<string?> Handle(UploadProductVariantImage request, CancellationToken cancellationToken)
        {
            /*
            var item = await _context.Products
                .Include(x => x.Variants)
                .FirstAsync(x => x.Id == request.ProductId);

            var variant = item.Variants
                .First(x => x.Id == request.VariantId);

            var blobId = $"{variant.Id}:{request.FileName}";

            await _blobStorageService.DeleteBlobAsync(blobId);

            await _blobStorageService.UploadBlobAsync(blobId, request.Stream);

            variant.Image = blobId;

            await _context.SaveChangesAsync();

            return GetImageUrl(variant.Image);
            */

            return string.Empty;
        }

        private static string? GetImageUrl(string? name)
        {
            return name is null ? null : $"http://127.0.0.1:10000/devstoreaccount1/images/{name}";
        }
    }
}