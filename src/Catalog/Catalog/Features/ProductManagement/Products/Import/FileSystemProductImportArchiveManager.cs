using System;
using System.IO;
using System.IO.Compression;
using System.Reflection;

namespace YourBrand.Catalog.Features.ProductManagement.Import;

public sealed class FileSystemProductImportArchiveManager : IProductImportArchiveManager
{
    public async Task<IProductImportArchive> CreateArchive(Stream stream, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var uploadsPath = GetUploadsPath();
        Directory.CreateDirectory(uploadsPath);

        var archiveName = DateTime.UtcNow.Ticks.ToString();
        var archiveDirectory = Path.Combine(uploadsPath, archiveName);
        Directory.CreateDirectory(archiveDirectory);

        var archiveFile = Path.Combine(uploadsPath, $"{archiveName}.zip");

        await using (var file = File.Open(archiveFile, FileMode.Create, FileAccess.ReadWrite, FileShare.None))
        {
            await stream.CopyToAsync(file, cancellationToken);
        }

        ZipFile.ExtractToDirectory(archiveFile, archiveDirectory);

        File.Delete(archiveFile);

        return new FileSystemProductImportArchive(archiveDirectory);
    }

    private static string GetUploadsPath()
    {
        var assemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
        return Path.Combine(assemblyLocation, "uploads");
    }

    private sealed class FileSystemProductImportArchive(string rootPath) : IProductImportArchive
    {
        public Task<Stream> OpenFileAsync(string relativePath, CancellationToken cancellationToken)
        {
            var fullPath = Path.Combine(rootPath, relativePath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"File '{relativePath}' not found in archive.", fullPath);
            }

            Stream stream = File.Open(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult(stream);
        }

        public Task DeleteFileAsync(string relativePath, CancellationToken cancellationToken)
        {
            var fullPath = Path.Combine(rootPath, relativePath);

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            return Task.CompletedTask;
        }

        public ValueTask DisposeAsync()
        {
            if (Directory.Exists(rootPath))
            {
                Directory.Delete(rootPath, true);
            }

            return ValueTask.CompletedTask;
        }
    }
}
