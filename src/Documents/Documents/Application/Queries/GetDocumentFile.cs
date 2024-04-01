using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Documents.Application.Services;
using YourBrand.Documents.Infrastructure.Persistence;

namespace YourBrand.Documents.Application.Queries;

public record GetDocumentFile(string DocumentId) : IRequest<DocumentFileResponse?>
{
    public class Handler : IRequestHandler<GetDocumentFile, DocumentFileResponse?>
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

        public async Task<DocumentFileResponse?> Handle(GetDocumentFile request, CancellationToken cancellationToken)
        {
            var document = await _context.Documents
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.DocumentId, cancellationToken);

            var stream = await _fileUploaderService.DownloadFileAsync(document!.BlobId);

            return document is null
                ? null
                : new DocumentFileResponse($"{document.Name}.{document.Extension}", document.ContentType, stream);
        }
    }
}

public record DocumentFileResponse(string FileName, string ContentType, Stream Stream);