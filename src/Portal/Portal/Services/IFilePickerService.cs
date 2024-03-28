namespace YourBrand.Portal.Services;

public interface IFilePickerService
{
    Task<Stream?> PickImage();
}