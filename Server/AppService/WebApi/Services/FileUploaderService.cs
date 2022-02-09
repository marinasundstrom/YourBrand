using Azure.Storage.Blobs;

using Catalog.Application.Common.Interfaces;

namespace Catalog.WebApi.Services;

public class FileUploaderService : IFileUploaderService
{
    private readonly BlobServiceClient _blobServiceClient;

    public FileUploaderService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task UploadFileAsync(string id, Stream stream, CancellationToken cancellationToken = default)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient("images");

        var response = await blobContainerClient.UploadBlobAsync(id, stream, cancellationToken);
    }
}