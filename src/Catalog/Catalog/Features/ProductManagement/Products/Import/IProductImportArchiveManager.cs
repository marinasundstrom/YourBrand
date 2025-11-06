using System.IO;

namespace YourBrand.Catalog.Features.ProductManagement.Import;

public interface IProductImportArchiveManager
{
    Task<IProductImportArchive> CreateArchive(Stream stream, CancellationToken cancellationToken);
}

public interface IProductImportArchive : IAsyncDisposable
{
    Task<Stream> OpenFileAsync(string relativePath, CancellationToken cancellationToken);

    Task DeleteFileAsync(string relativePath, CancellationToken cancellationToken);
}
