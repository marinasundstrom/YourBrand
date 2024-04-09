using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Documents.Domain;

namespace YourBrand.Documents.Application.Commands;

public record DeleteDocument(string DocumentId) : IRequest
{
    public class Handler(IDocumentsContext context) : IRequestHandler<DeleteDocument>
    {
        public async Task Handle(DeleteDocument request, CancellationToken cancellationToken)
        {
            var document = await context.Documents
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.DocumentId, cancellationToken);

            if (document is null)
            {
                throw new Exception("Document not found.");
            }

            context.Documents.Remove(document);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}