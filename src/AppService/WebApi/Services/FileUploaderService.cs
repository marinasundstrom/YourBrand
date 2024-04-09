using Azure.Storage.Blobs;

using YourBrand.Application.Common.Interfaces;

namespace YourBrand.WebApi.Services;

public class FileUploaderService(BlobServiceClient blobServiceClient) : IFileUploaderService
{
    public async Task UploadFileAsync(string id, Stream stream, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("images");

        var response = await blobContainerClient.UploadBlobAsync(id, stream, cancellationToken);
    }
}