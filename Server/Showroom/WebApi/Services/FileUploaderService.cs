using Azure.Storage.Blobs;

using YourCompany.Showroom.Application.Common.Interfaces;

namespace YourCompany.Showroom.WebApi.Services;

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

        await blobContainerClient.DeleteBlobIfExistsAsync(id);

        var response = await blobContainerClient.UploadBlobAsync(id, stream, cancellationToken);
    }
}