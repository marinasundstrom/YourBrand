using YourBrand.Documents.Infrastructure.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Documents.Domain;

namespace YourBrand.Documents.Application.Commands;

public record DeleteDocument(string DocumentId) : IRequest
{
    public class Handler : IRequestHandler<DeleteDocument>
    {
        private readonly IDocumentsContext _context;

        public Handler(IDocumentsContext context)
        {
            _context = context;
        }

        public async Task<Unit> Handle(DeleteDocument request, CancellationToken cancellationToken)
        {
            var document = await _context.Documents
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.DocumentId, cancellationToken);

            if (document is null)
            {
                throw new Exception("Document not found.");
            }

            _context.Documents.Remove(document);

            await _context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}