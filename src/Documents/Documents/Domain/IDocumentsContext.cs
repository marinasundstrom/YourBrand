using Microsoft.EntityFrameworkCore;

using YourBrand.Documents.Domain.Entities;

using Directory = YourBrand.Documents.Domain.Entities.Directory;

namespace YourBrand.Documents.Domain;

public interface IDocumentsContext
{
    DbSet<Directory> Directories { get; }

    DbSet<Document> Documents { get; }

    DbSet<DocumentType> DocumentTypes { get; }

    DbSet<DocumentTemplate> DocumentTemplates { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}