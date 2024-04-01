using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Documents.Domain;

namespace YourBrand.Documents.Application.Commands;

public record CheckNameTaken(string Name) : IRequest<bool>
{
    public class Handler : IRequestHandler<CheckNameTaken, bool>
    {
        private readonly IDocumentsContext _context;

        public Handler(IDocumentsContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(CheckNameTaken request, CancellationToken cancellationToken)
        {
            var n = request.Name;

            var document = await _context.Documents
             .AsSplitQuery()
             .AsNoTracking()
             .FirstOrDefaultAsync(x => x.Name == n, cancellationToken);

            return document is not null;
        }
    }
}