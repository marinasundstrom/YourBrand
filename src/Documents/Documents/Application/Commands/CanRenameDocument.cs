using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Documents.Domain;

namespace YourBrand.Documents.Application.Commands;

public record CanRenameDocument(string DocumentId, string NewName) : IRequest<bool>
{
    public class Handler : IRequestHandler<CanRenameDocument, bool>
    {
        private readonly IDocumentsContext _context;

        public Handler(IDocumentsContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CanRenameDocument request, CancellationToken cancellationToken)
        {
            var document = await _context.Documents
             .Include(x => x.Directory)
             .AsSplitQuery()
             .AsNoTracking()
             .FirstOrDefaultAsync(x => x.Id == request.DocumentId, cancellationToken);

            var document2 = await _context.Documents
             .AsSplitQuery()
             .AsNoTracking()
             .Where(x => x.DirectoryId == document.DirectoryId)
             .FirstOrDefaultAsync(x => x.Id != request.DocumentId && x.Name == request.NewName, cancellationToken);

            return document2 is null;
        }
    }
}