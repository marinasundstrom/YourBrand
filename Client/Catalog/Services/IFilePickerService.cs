namespace Catalog.Services;

public interface IFilePickerService
{
    Task<Stream?> PickImage();
}