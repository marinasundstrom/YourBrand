using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Documents.Domain;

namespace YourBrand.Documents.Application.Commands;

public record CheckNameTaken(string Name) : IRequest<bool>
{
    public class Handler(IDocumentsContext context) : IRequestHandler<CheckNameTaken, bool>
    {
        public async Task<bool> Handle(CheckNameTaken request, CancellationToken cancellationToken)
        {
            var n = request.Name;

            var document = await context.Documents
             .AsSplitQuery()
             .AsNoTracking()
             .FirstOrDefaultAsync(x => x.Name == n, cancellationToken);

            return document is not null;
        }
    }
}