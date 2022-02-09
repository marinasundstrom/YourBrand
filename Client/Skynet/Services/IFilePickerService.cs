namespace Skynet.Services;

public interface IFilePickerService
{
    Task<Stream?> PickImage();
}