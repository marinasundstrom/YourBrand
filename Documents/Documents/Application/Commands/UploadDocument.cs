using YourBrand.Documents.Application.Services;
using YourBrand.Documents.Infrastructure.Persistence;

using MediatR;
using YourBrand.Documents.Domain;
using YourBrand.Documents.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Documents.Application.Commands;

public record UploadDocument(string Name, string ContentType, Stream Stream) : IRequest<DocumentDto>
{
    public class Handler : IRequestHandler<UploadDocument, DocumentDto>
    {
        private readonly IDocumentsContext _context;
        private readonly IFileUploaderService _fileUploaderService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(IDocumentsContext context, IFileUploaderService fileUploaderService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _fileUploaderService = fileUploaderService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DocumentDto> Handle(UploadDocument request, CancellationToken cancellationToken)
        {
            var n = Path.GetFileNameWithoutExtension(request.Name);

            var document = await _context.Documents
             .AsSplitQuery()
             .AsNoTracking()
             .FirstOrDefaultAsync(x => x.Name == n, cancellationToken);

            string name = request.Name;
            string contentType = request.ContentType;

            if (document is not null)
            {
                var e = Path.GetExtension(request.Name);
                name = $"{n} (2){e}";
            }

            document = new Domain.Entities.Document(name, contentType);

            _context.Documents.Add(document);

            document.BlobId = $"{document.Id.Replace("-", string.Empty)}";

            await _fileUploaderService.UploadFileAsync(document.BlobId, request.Stream, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return document.ToDto(GetUrl(document));
        }

        private string GetUrl(Document document)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            return $"{request.Scheme}://{request.Host}/Documents/{document.Id}/File";

            //return $"{request.Scheme}://{request.Host}/content/documents/{blobId}";
        }
    }
}
