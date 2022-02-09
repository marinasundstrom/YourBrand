using System;

using Azure.Storage.Blobs;

using Skynet.TimeReport.Application.Common.Interfaces;

namespace Skynet.TimeReport.Services;

class BlobService : IBlobService
{
    private readonly BlobServiceClient _blobServiceClient;

    public BlobService(BlobServiceClient blobServiceClient)
    {
        _blobServiceClient = blobServiceClient;
    }

    public async Task UploadBloadAsync(string name, Stream stream)
    {
        var blobContainerClient = _blobServiceClient.GetBlobContainerClient("attachments");

#if DEBUG
        await blobContainerClient.CreateIfNotExistsAsync();
#endif

        var response = await blobContainerClient.UploadBlobAsync(name, stream);
    }
}