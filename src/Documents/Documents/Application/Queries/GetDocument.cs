using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Documents.Infrastructure.Persistence;

namespace YourBrand.Documents.Application.Queries;

public record GetDocument(string DocumentId) : IRequest<DocumentDto?>
{
    public class Handler : IRequestHandler<GetDocument, DocumentDto?>
    {
        private readonly DocumentsContext _context;
        private readonly IUrlResolver _urlResolver;

        public Handler(DocumentsContext context, IUrlResolver urlResolver)
        {
            _context = context;
            _urlResolver = urlResolver;
        }

        public async Task<DocumentDto?> Handle(GetDocument request, CancellationToken cancellationToken)
        {
            var document = await _context.Documents
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.DocumentId, cancellationToken);

            return document is null
                ? null
                : document.ToDto(_urlResolver.GetUrl);
        }
    }
}