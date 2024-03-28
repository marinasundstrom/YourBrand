using YourBrand.Documents.Infrastructure.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Documents.Domain;

namespace YourBrand.Documents.Application.Commands;

public record RenameDocument(string DocumentId, string NewName) : IRequest
{
    public class Handler : IRequestHandler<RenameDocument>
    {
        private readonly IDocumentsContext _context;

        public Handler(IDocumentsContext context)
        {
            _context = context;
        }

        public async Task Handle(RenameDocument request, CancellationToken cancellationToken)
        {
            var document = await _context.Documents
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.DocumentId, cancellationToken);

            if(document is null) 
            {
                throw new Exception("Document not found.");
            }

            document.Rename(request.NewName);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}
