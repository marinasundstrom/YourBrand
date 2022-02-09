using System;

using Foundation;

using UIKit;

namespace Catalog.MacCatalyst;

public static class ImageConverter
{
    public static async Task<Stream> ConvertToJpegAsync(Stream stream)
    {
        using var imageData = NSData.FromStream(stream);
        using var image = UIImage.LoadFromData(imageData);
        using var jpg = image.AsJPEG(0.5f);

        MemoryStream memoryStream = new();
        await jpg.AsStream().CopyToAsync(memoryStream);
        memoryStream.Seek(0, SeekOrigin.Begin);

        return memoryStream;
    }
}