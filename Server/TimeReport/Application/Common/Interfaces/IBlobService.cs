namespace TimeReport.Application.Common.Interfaces;

public interface IBlobService
{
    Task UploadBloadAsync(string name, Stream stream);
}