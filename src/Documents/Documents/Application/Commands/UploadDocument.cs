using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Documents.Application.Queries;
using YourBrand.Documents.Application.Services;
using YourBrand.Documents.Domain;

namespace YourBrand.Documents.Application.Commands;

public record UploadDocument(string Name, string ContentType, Stream Stream, string DirectoryId) : IRequest<DocumentDto>
{
    public class Handler : IRequestHandler<UploadDocument, DocumentDto>
    {
        private readonly IDocumentsContext _context;
        private readonly IFileUploaderService _fileUploaderService;
        private readonly IUrlResolver _urlResolver;

        public Handler(IDocumentsContext context, IFileUploaderService fileUploaderService, IUrlResolver urlResolver)
        {
            _context = context;
            _fileUploaderService = fileUploaderService;
            _urlResolver = urlResolver;
        }

        public async Task<DocumentDto> Handle(UploadDocument request, CancellationToken cancellationToken)
        {
            var n = Path.GetFileNameWithoutExtension(request.Name);

            var document = await _context.Documents
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

            var directory = await _context.Directories
             .Include(x => x.Documents)
             .AsSplitQuery()
             .FirstAsync(/* x => x.Id == request.DirectoryId, */ cancellationToken);

            document = new Domain.Entities.Document(name, contentType);

            directory.AddDocument(document);

            document.BlobId = $"{document.Id.Replace("-", string.Empty)}";

            await _fileUploaderService.UploadFileAsync(document.BlobId, request.Stream, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);

            return document.ToDto(_urlResolver.GetUrl);
        }
    }
}