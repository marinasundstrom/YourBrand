using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Documents.Infrastructure.Persistence;

namespace YourBrand.Documents.Application.Queries;

public record GetDirectory(string Path) : IRequest<DirectoryDto?>
{
    public class Handler(DocumentsContext context, IUrlResolver urlResolver) : IRequestHandler<GetDirectory, DirectoryDto?>
    {
        public async Task<DirectoryDto?> Handle(GetDirectory request, CancellationToken cancellationToken)
        {
            var path = request.Path?.Trim() ?? string.Empty;

            var directory = await context.Directories
                .Include(x => x.Parent)
                .Include(x => x.Directories)
                .Include(x => x.Documents)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Name == path, cancellationToken);

            return directory is null
                ? null
                : directory.ToDto(urlResolver.GetUrl);
        }
    }
}