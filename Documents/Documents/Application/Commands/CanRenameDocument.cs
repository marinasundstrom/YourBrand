using YourBrand.Documents.Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
             .AsSplitQuery()
             .AsNoTracking()
             .FirstOrDefaultAsync(x => x.Id != request.DocumentId && x.Name == request.NewName, cancellationToken);

            return document is null;
        }
    }
}