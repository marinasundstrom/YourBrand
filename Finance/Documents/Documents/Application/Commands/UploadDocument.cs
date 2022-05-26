using Documents.Application.Services;
using Documents.Infrastructure.Persistence;

using MediatR;

namespace Documents.Application.Commands;

public record UploadDocument(string Title, string ContentType, Stream Stream) : IRequest<DocumentDto>
{
    public class Handler : IRequestHandler<UploadDocument, DocumentDto>
    {
        private readonly DocumentsContext _context;
        private readonly IFileUploaderService _fileUploaderService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(DocumentsContext context, IFileUploaderService fileUploaderService, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _fileUploaderService = fileUploaderService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DocumentDto> Handle(UploadDocument request, CancellationToken cancellationToken)
        {
            var document = new Domain.Entities.Document(request.Title, request.ContentType);

            _context.Documents.Add(document);

            document.BlobId = $"{document.Id.Replace("-", string.Empty)}";

            await _fileUploaderService.UploadFileAsync(document.BlobId, request.Stream, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return document.ToDto(GetUrl(document.BlobId));
        }

         private string GetUrl(string blobId)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            return $"{request.Scheme}://{request.Host}/documents/{blobId}";
        }
    }
}