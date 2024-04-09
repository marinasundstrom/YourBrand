using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Documents.Infrastructure.Persistence;

namespace YourBrand.Documents.Application.Queries;

public record GetDocument(string DocumentId) : IRequest<DocumentDto?>
{
    public class Handler(DocumentsContext context, IUrlResolver urlResolver) : IRequestHandler<GetDocument, DocumentDto?>
    {
        public async Task<DocumentDto?> Handle(GetDocument request, CancellationToken cancellationToken)
        {
            var document = await context.Documents
                .AsSplitQuery()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.DocumentId, cancellationToken);

            return document is null
                ? null
                : document.ToDto(urlResolver.GetUrl);
        }
    }
}