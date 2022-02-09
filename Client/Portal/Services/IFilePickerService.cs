namespace Skynet.Portal.Services;

public interface IFilePickerService
{
    Task<Stream?> PickImage();
}