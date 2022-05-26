
namespace Documents.Application.Services
{
    public interface IPdfGenerator
    {
        Task<Stream> GeneratePdfFromHtmlAsync(string html, Uri? baseUrlOrPath = null);
    }
}