using Documents.Infrastructure.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Documents.Application.Commands;

public record RenameDocument(string DocumentId, string NewName) : IRequest
{
    public class Handler : IRequestHandler<RenameDocument>
    {
        private readonly DocumentsContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public Handler(DocumentsContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Unit> Handle(RenameDocument request, CancellationToken cancellationToken)
        {
            var document = await _context.Documents
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.DocumentId, cancellationToken);

            if(document is null) 
            {
                throw new Exception("Document not found.");
            }

            document.Rename(request.NewName);

            return Unit.Value;

            /*
            return document is null
                ? null
                : document.ToDto(GetUrl(document.BlobId)); */
        }

        private string GetUrl(string blobId)
        {
            var request = _httpContextAccessor.HttpContext!.Request;

            return $"{request.Scheme}://{request.Host}/documents/{blobId}";
        }
    }
}