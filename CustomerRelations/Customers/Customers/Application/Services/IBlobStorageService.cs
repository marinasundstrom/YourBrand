namespace YourBrand.Customers.Application.Services;

public interface IBlobStorageService
{
    Task<Stream> GetBlobAsync(string id);

    Task UploadBlobAsync(string id, Stream stream);

    Task DeleteBlobAsync(string id);
}
