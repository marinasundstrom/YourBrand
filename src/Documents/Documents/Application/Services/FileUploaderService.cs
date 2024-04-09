using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace YourBrand.Documents.Application.Services;

public class FileUploaderService(BlobServiceClient blobServiceClient) : IFileUploaderService
{
    private bool flag;

    public async Task DeleteFileAsync(string id, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("documents");

        if (!flag)
        {
            await blobContainerClient.CreateIfNotExistsAsync();
            flag = true;
        }

        var response = await blobContainerClient.DeleteBlobIfExistsAsync(id);
    }

    public async Task<Stream> DownloadFileAsync(string blobId)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("documents");

        if (!flag)
        {
            await blobContainerClient.CreateIfNotExistsAsync();
            flag = true;
        }

        var blobClient = blobContainerClient.GetBlobClient(blobId);

        return await blobClient.OpenReadAsync(new BlobOpenReadOptions(false), default);
    }

    public async Task UploadFileAsync(string id, Stream stream, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = blobServiceClient.GetBlobContainerClient("documents");

        if (!flag)
        {
            await blobContainerClient.CreateIfNotExistsAsync();
            flag = true;
        }

        var response = await blobContainerClient.UploadBlobAsync(id, stream, cancellationToken);
    }
}