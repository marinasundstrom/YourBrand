namespace YourCompany.Portal.Services;

public interface IFilePickerService
{
    Task<Stream?> PickImage();
}