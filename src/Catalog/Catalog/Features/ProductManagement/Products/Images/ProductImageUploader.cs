using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

using YourBrand.Catalog.Persistence;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.ProductManagement.Products.Images;

public interface IProductImageUploader
{
    Task<string> GetPlaceholderImageUrl();
    Task<bool> TryDeleteProductImage(long productId, string fileName);
    Task<string> UploadProductImage(long productId, string fileName, Stream stream, string contentType);
}

public class ProductImageUploader(BlobServiceClient blobServiceClient, CatalogContext context, IConfiguration configuration)
    : IProductImageUploader
{
    private string? placeholderImageFileName;

    public Task<string> GetPlaceholderImageUrl()
    {
        if (placeholderImageFileName is null)
        {
            string fileName = "placeholder.jpeg";

            string? productImageUrlPath = configuration.GetValue<string>("ImagesUrlPath");

            var path = $"products/{fileName}";

            placeholderImageFileName = string.Format(productImageUrlPath!, path); ;
        }

        return Task.FromResult(placeholderImageFileName);
    }

    public async Task<bool> TryDeleteProductImage(long productId, string fileName)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("images");
        await blobContainerClient.CreateIfNotExistsAsync();

        BlobClient blobClient = blobContainerClient.GetBlobClient($"products/{productId}/{fileName}");

        return await blobClient.DeleteIfExistsAsync();
    }

    public async Task<string> UploadProductImage(long productId, string fileName, Stream stream, string contentType)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("images");
        await blobContainerClient.CreateIfNotExistsAsync();

        BlobClient blobClient = blobContainerClient.GetBlobClient($"products/{productId}/{fileName}");

        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = contentType });

        string? productImageUrlPath = configuration.GetValue<string>("ImagesUrlPath");

        var path = $"products/{productId}/{fileName}";

        return string.Format(productImageUrlPath!, path);
    }
}