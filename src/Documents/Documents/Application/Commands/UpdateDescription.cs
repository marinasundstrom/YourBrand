using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Documents.Domain;

namespace YourBrand.Documents.Application.Commands;

public record UpdateDescription(string DocumentId, string Description) : IRequest
{
    public class Handler : IRequestHandler<UpdateDescription>
    {
        private readonly IDocumentsContext _context;

        public Handler(IDocumentsContext context)
        {
            _context = context;
        }

        public async Task Handle(UpdateDescription request, CancellationToken cancellationToken)
        {
            var document = await _context.Documents
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == request.DocumentId, cancellationToken);

            if (document is null)
            {
                throw new Exception("Document not found.");
            }

            document.UpdateDescription(request.Description);

            await _context.SaveChangesAsync(cancellationToken);

        }
    }
}