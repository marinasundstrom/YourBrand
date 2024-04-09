using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Documents.Application.Queries;
using YourBrand.Documents.Application.Services;
using YourBrand.Documents.Domain;

namespace YourBrand.Documents.Application.Commands;

public record UploadDocument(string Name, string ContentType, Stream Stream, string DirectoryId) : IRequest<DocumentDto>
{
    public class Handler(IDocumentsContext context, IFileUploaderService fileUploaderService, IUrlResolver urlResolver) : IRequestHandler<UploadDocument, DocumentDto>
    {
        public async Task<DocumentDto> Handle(UploadDocument request, CancellationToken cancellationToken)
        {
            var n = Path.GetFileNameWithoutExtension(request.Name);

            var document = await context.Documents
             .AsSplitQuery()
             .AsNoTracking()
             .Where(x => x.DirectoryId == request.DirectoryId)
             .FirstOrDefaultAsync(x => x.Name == n, cancellationToken);

            string name = request.Name;
            string contentType = request.ContentType;

            if (document is not null)
            {
                var e = Path.GetExtension(request.Name);
                name = $"{n} (2){e}";
            }

            var directory = await context.Directories
             .Include(x => x.Documents)
             .AsSplitQuery()
             .FirstAsync(/* x => x.Id == request.DirectoryId, */ cancellationToken);

            document = new Domain.Entities.Document(name, contentType);

            directory.AddDocument(document);

            document.BlobId = $"{document.Id.Replace("-", string.Empty)}";

            await fileUploaderService.UploadFileAsync(document.BlobId, request.Stream, cancellationToken);

            await context.SaveChangesAsync(cancellationToken);

            return document.ToDto(urlResolver.GetUrl);
        }
    }
}