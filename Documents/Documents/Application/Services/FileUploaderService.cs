using Azure.Storage.Blobs;

using YourBrand.Documents.Application.Services;

namespace YourBrand.Documents.Application.Services;

public class FileUploaderService : IFileUploaderService
{
    private readonly BlobServiceClient _blobServiceClient;
    private bool flag;

    public FileUploaderService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task DeleteFileAsync(string id, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient("documents");

        if (!flag)
        {
            await blobContainerClient.CreateIfNotExistsAsync();
            flag = true;
        }

        var response = await blobContainerClient.DeleteBlobIfExistsAsync(id);
    }

    public async Task UploadFileAsync(string id, Stream stream, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient("documents");


        if (!flag)
        {
            await blobContainerClient.CreateIfNotExistsAsync();
            flag = true;
        }
        var response = await blobContainerClient.UploadBlobAsync(id, stream, cancellationToken);
    }
}