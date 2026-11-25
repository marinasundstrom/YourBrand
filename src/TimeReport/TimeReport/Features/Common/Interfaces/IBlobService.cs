namespace YourBrand.TimeReport.Features.Common.Interfaces;

public interface IBlobService
{
    Task UploadBloadAsync(string name, Stream stream);
}