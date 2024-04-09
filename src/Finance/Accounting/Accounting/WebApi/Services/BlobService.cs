using Azure.Storage.Blobs;

using YourBrand.Accounting.Application.Common.Interfaces;

namespace YourBrand.Accounting.Services;

class BlobService(BlobServiceClient blobServiceClient) : IBlobService
{
    public async Task UploadBloadAsync(string name, Stream stream)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("attachments");

#if DEBUG
        await blobContainerClient.CreateIfNotExistsAsync();
#endif

        var response = await blobContainerClient.UploadBlobAsync(name, stream);
    }
}