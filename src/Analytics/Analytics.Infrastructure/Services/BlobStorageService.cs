using Azure.Storage.Blobs;

namespace YourBrand.Analytics.Infrastructure.Services;

public sealed class BlobStorageService(BlobServiceClient blobServiceClient) : IBlobStorageService
{
    public async Task DeleteBlobAsync(string id)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("images");

#if DEBUG
        await blobContainerClient.CreateIfNotExistsAsync();
#endif

        await blobContainerClient.DeleteBlobIfExistsAsync(id);
    }

    public async Task<Stream> GetBlobAsync(string id)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("images");

#if DEBUG
        await blobContainerClient.CreateIfNotExistsAsync();
#endif

        var blobClient = blobContainerClient.GetBlobClient(id);
        return await blobClient.OpenReadAsync();
    }

    public async Task UploadBlobAsync(string id, Stream stream)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("images");

#if DEBUG
        await blobContainerClient.CreateIfNotExistsAsync();
#endif

        await blobContainerClient.UploadBlobAsync(id, stream);
    }
}