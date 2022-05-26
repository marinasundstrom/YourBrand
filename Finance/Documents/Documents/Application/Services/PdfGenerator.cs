using IronPdf;

namespace Documents.Application.Services;

public class PdfGenerator : IPdfGenerator
{
    readonly HtmlToPdf renderer = new HtmlToPdf();

    public async Task<Stream> GeneratePdfFromHtmlAsync(string html, Uri? baseUrlOrPath = null)
    {
        var doc = await renderer.RenderHtmlAsPdfAsync(html, baseUrlOrPath);

        return doc.Stream;
    }
}