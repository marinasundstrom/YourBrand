using System;

namespace Skynet.Application.Common.Interfaces;

public interface IFileUploaderService
{
    Task UploadFileAsync(string id, Stream stream, CancellationToken cancellationToken = default);
}