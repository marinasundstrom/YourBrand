using Documents.Infrastructure.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Documents.Application.Queries;

public record GetDocument(string DocumentId) : IRequest<DocumentDto?>
{
    public class Handler : IRequestHandler<GetDocument, DocumentDto?>
    {
        private readonly DocumentsContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(DocumentsContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<DocumentDto?> Handle(GetDocument request, CancellationToken cancellationToken)
        {
            var document = await _context.Documents
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.DocumentId, cancellationToken);

            return document is null
                ? null
                : document.ToDto(GetUrl(document.BlobId));
        }

        private string GetUrl(string blobId)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            return $"{request.Scheme}://{request.Host}/content/documents/{blobId}";
        }
    }
}