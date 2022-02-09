using System;
namespace Skynet.Portal.Services;

public class FilePickerService : IFilePickerService
{
    public Task<Stream?> PickImage()
    {
        return Task.FromResult<Stream?>(null!);
    }
}