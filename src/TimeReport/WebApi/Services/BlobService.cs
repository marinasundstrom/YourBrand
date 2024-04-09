using Azure.Storage.Blobs;

using YourBrand.TimeReport.Application.Common.Interfaces;

namespace YourBrand.TimeReport.Services;

sealed class BlobService(BlobServiceClient blobServiceClient) : IBlobService
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