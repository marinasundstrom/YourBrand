using YourBrand.Customers.Application.Services;
using Azure.Storage.Blobs;

namespace YourBrand.Customers.Infrastructure.Services;

public sealed class BlobStorageService : IBlobStorageService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobStorageService(BlobServiceClient blobServiceClient)
    {
        this._blobServiceClient = blobServiceClient;
    }

    public async Task DeleteBlobAsync(string id)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient("images");

#if DEBUG
        await blobContainerClient.CreateIfNotExistsAsync();
#endif

        await blobContainerClient.DeleteBlobIfExistsAsync(id);
    }

    public async Task<Stream> GetBlobAsync(string id)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient("images");

#if DEBUG
        await blobContainerClient.CreateIfNotExistsAsync();
#endif

        var blobClient = blobContainerClient.GetBlobClient(id);
        return await blobClient.OpenReadAsync();
    }

    public async Task UploadBlobAsync(string id, Stream stream)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient("images");

#if DEBUG
        await blobContainerClient.CreateIfNotExistsAsync();
#endif

        await blobContainerClient.UploadBlobAsync(id, stream);
    }
}