using Azure.Storage.Blobs;

using YourBrand.Showroom.Application.Common.Interfaces;

namespace YourBrand.Showroom.WebApi.Services;

public class FileUploaderService(BlobServiceClient blobServiceClient) : IFileUploaderService
{
    public async Task UploadFileAsync(string id, Stream stream, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("images");

        await blobContainerClient.DeleteBlobIfExistsAsync(id);

        var response = await blobContainerClient.UploadBlobAsync(id, stream, cancellationToken);
    }
}